using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineEvaluation.Api.Constants;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class StudentOnboardingService : IStudentOnboardingService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentOnboardingService> _logger;

        public StudentOnboardingService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<StudentOnboardingService> logger)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BulkOperationResultDto<BulkRowErrorDto>> RegisterBulkStudentsAsync(
            List<StudentRegistrationDto> dtos,
            string actorUserId)
        {
            var result = new BulkOperationResultDto<BulkRowErrorDto>
            {
                TotalRecordsReceived = dtos?.Count ?? 0,
                Errors = new List<BulkRowErrorDto>()
            };

            if (dtos == null || !dtos.Any())
                return result;

            var academicMapsMatrix = await _db.Set<AcademicMap>()
                .Include(m => m.StudyProgram)
                .AsNoTracking()
                .ToDictionaryAsync(m => m.Id);

            const int BatchSize = 100;
            int totalRecords = dtos.Count;

            for (int batchStart = 0; batchStart < totalRecords; batchStart += BatchSize)
            {
                var currentBatchChunk = dtos.Skip(batchStart).Take(BatchSize).ToList();
                var identityAccountsCreatedInBatch = new List<ApplicationUser>();

                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    for (int i = 0; i < currentBatchChunk.Count; i++)
                    {
                        var dto = currentBatchChunk[i];

                        int currentSpreadsheetRow = batchStart + i + 2;
                        string currentRegNo = dto.RegistrationNumber?.Trim().ToUpper() ?? "UNKNOWN";

                        if (!academicMapsMatrix.TryGetValue(dto.AcademicMapId, out var cachedMap))
                        {
                            throw new InvalidOperationException(
                                $"Row {currentSpreadsheetRow} [RegNo: {currentRegNo}]: AcademicMap identity ({dto.AcademicMapId}) was not found.");
                        }

                        string cleanFirstName = dto.FirstName?.Replace(" ", "").Trim() ?? "Student";
                        string formattedDob = dto.DateOfBirth.ToString("ddMMyyyy");
                        string evaluatedBulkPassword = string.IsNullOrWhiteSpace(dto.Password)
                            ? $"{cleanFirstName}@{formattedDob}"
                            : dto.Password;

                        var createdUser = await StageOnboardingCoreAsync(dto, evaluatedBulkPassword, actorUserId, cachedMap);
                        identityAccountsCreatedInBatch.Add(createdUser);
                    }

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    result.SuccessfullyProcessedCount += currentBatchChunk.Count;

                    _db.ChangeTracker.Clear();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _db.ChangeTracker.Clear();

                    _logger.LogError(ex, "Batch chunk processing exception caught between spreadsheet records row context {StartRow} and {EndRow}.",
                        batchStart + 2, batchStart + currentBatchChunk.Count + 1);


                    foreach (var user in identityAccountsCreatedInBatch)
                    {
                        try
                        {
                            var existingUser = await _userManager.FindByIdAsync(user.Id);
                            if (existingUser != null)
                            {
                                await _userManager.DeleteAsync(existingUser);
                            }
                        }
                        catch (Exception deleteEx)
                        {
                            _logger.LogCritical(deleteEx, "Critical: Leaked Identity registration record cleanup error for username target {UserName}.", user.UserName);
                        }
                    }

                    result.Errors.Add(new BulkRowErrorDto
                    {
                        RowNumber = batchStart + 2,
                        Identifier = $"BATCH_FAILURE_{batchStart}",
                        ErrorMessage = $"Chunk block beginning at spreadsheet row position context {batchStart + 2} aborted: {ex.InnerException?.Message ?? ex.Message}"
                    });
                }
            }

            return result;
        }

        public async Task<StudentDto> RegisterSingleStudentAsync(
            StudentRegistrationDto dto,
            string actorUserId)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("An explicit initial account credentials password is required for single form registration.");

            var academicMap = await _db.Set<AcademicMap>()
                .Include(m => m.StudyProgram)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.AcademicMapId);

            if (academicMap == null)
                throw new KeyNotFoundException(
                    $"Onboarding aborted: The Academic Map profile ID ({dto.AcademicMapId}) does not exist.");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var coreUser = await StageOnboardingCoreAsync(dto, dto.Password, actorUserId, academicMap);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var targetStudent = _db.Students.Local.First(s => s.ApplicationUserId == coreUser.Id);
                return _mapper.Map<StudentDto>(targetStudent);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _db.ChangeTracker.Clear();

                _logger.LogError(ex, "Single onboarding failed for RegNo {RegNo}", dto.RegistrationNumber);

                var cleanRegNo = dto.RegistrationNumber?.Trim().ToUpper();
                var leakedUser = await _userManager.FindByNameAsync(cleanRegNo);
                if (leakedUser != null)
                {
                    await _userManager.DeleteAsync(leakedUser);
                }
                throw;
            }
        }

        private async Task<ApplicationUser> StageOnboardingCoreAsync(
            StudentRegistrationDto dto,
            string explicitPassword,
            string actorUserId,
            AcademicMap preLoadedMap)
        {
            if (string.IsNullOrWhiteSpace(dto.RegistrationNumber) || dto.RegistrationNumber.Trim().Length != 10)
                throw new ArgumentException("Registration number is invalid. It must be exactly 10 characters long.");

            string cleanRegNo = dto.RegistrationNumber.Trim().ToUpper();

            if (!int.TryParse(cleanRegNo.Substring(0, 2), out int parsedShortYear))
                throw new ArgumentException($"Registration number standard prefix '{cleanRegNo.Substring(0, 2)}' is not a valid year indicator.");

            int startYear = 2000 + parsedShortYear;

            if (preLoadedMap?.StudyProgram == null)
                throw new InvalidOperationException("The requested configuration academic profile trace is invalid or incomplete.");

            int endYear = startYear + preLoadedMap.StudyProgram.DurationInYears;
            string dynamicBatchTimeline = $"{startYear}-{endYear}";
            string baselineAcademicYear = $"{startYear}-{startYear + 1}";

            var coreIdentityAccount = new ApplicationUser
            {
                UserName = cleanRegNo,
                Email = dto.Email.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                IsActive = true,
                MustChangePassword = true,
                EmailConfirmed = true
            };

            var identityCreationResponse = await _userManager.CreateAsync(coreIdentityAccount, explicitPassword);
            if (!identityCreationResponse.Succeeded)
            {
                string pooledErrors = string.Join(" | ", identityCreationResponse.Errors.Select(e => e.Description));
                throw new InvalidOperationException(pooledErrors);
            }

            var roleResult = await _userManager.AddToRoleAsync(coreIdentityAccount, "Student");
            if (!roleResult.Succeeded)
            {
                string roleErrors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                await _userManager.DeleteAsync(coreIdentityAccount);
                throw new InvalidOperationException($"Identity Role Assignment Failed: {roleErrors}");
            }

            var studentDomainModel = _mapper.Map<Student>(dto);
            studentDomainModel.Guid = Guid.NewGuid();
            studentDomainModel.ApplicationUserId = coreIdentityAccount.Id;
            studentDomainModel.Batch = dynamicBatchTimeline;
            studentDomainModel.AcademicAliasCode = preLoadedMap.AliasCode;
            studentDomainModel.CreatedAt = DateTime.UtcNow;
            studentDomainModel.CreatedBy = actorUserId;

            _db.Students.Add(studentDomainModel);

            var baselineSemesterRecord = new StudentAcademicRecord
            {
                Student = studentDomainModel,
                AcademicMapId = preLoadedMap.Id,
                AcademicAliasCode = studentDomainModel.AcademicAliasCode,
                Semester = 1,
                AcademicYear = baselineAcademicYear,
                IsCurrentSemester = true,
                Standing = SemesterStanding.Active,
                AcademicSessionSlug = $"{preLoadedMap.Regulation}-{dynamicBatchTimeline}-SEM1",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actorUserId
            };

            _db.StudentAcademicRecords.Add(baselineSemesterRecord);

            return coreIdentityAccount;
        }
    }
}