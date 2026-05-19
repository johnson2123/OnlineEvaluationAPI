using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public StudentOnboardingService(ApplicationDbContext db,
                                        UserManager<ApplicationUser> userManager,
                                        IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<BulkOperationResultDto<BulkRowErrorDto>> RegisterBulkStudentsAsync(List<StudentRegistrationDto> dtos, string actorUserId)
        {
            var result = new BulkOperationResultDto<BulkRowErrorDto>
            {
                TotalRecordsReceived = dtos?.Count ?? 0,
                Errors = new List<BulkRowErrorDto>()
            };

            if (dtos == null || !dtos.Any())
                return result;

            // PERFORMANCE OPTIMIZATION: Cache configuration maps into memory lookups upfront
            var academicMapsMatrix = await _db.Set<AcademicMap>()
                .Include(m => m.StudyProgram)
                .AsNoTracking()
                .ToDictionaryAsync(m => m.Id);

            const int batchSize = 100;
            for (int i = 0; i < dtos.Count; i += batchSize)
            {
                var currentBatchChunk = dtos.Skip(i).Take(batchSize).ToList();

                // Open an isolated atomic transaction block for the current chunk
                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    for (int j = 0; j < currentBatchChunk.Count; j++)
                    {
                        var dto = currentBatchChunk[j];

                        // Calculate physical spreadsheet row number (assumes 1-based index + 1 header row)
                        int currentSpreadsheetRow = i + j + 2;
                        string currentRegNo = dto.RegistrationNumber?.Trim().ToUpper() ?? "UNKNOWN";

                        try
                        {

                            if (!academicMapsMatrix.TryGetValue(dto.AcademicMapId, out var cachedMap))
                            {
                                result.Errors.Add(new BulkRowErrorDto
                                {
                                    RowNumber = currentSpreadsheetRow,
                                    Identifier = $"RegNo: {currentRegNo}",
                                    ErrorMessage = $"AcademicMap reference identity ({dto.AcademicMapId}) was not found in the registry."
                                });
                                continue; // Move to the next student row without breaking the batch loop
                            }

                            string cleanFirstName = dto.FirstName?.Replace(" ", "").Trim() ?? "Student";
                            string formattedDob = dto.DateOfBirth.ToString("ddMMyyyy");
                            string evaluatedBulkPassword = string.IsNullOrWhiteSpace(dto.Password)
                                                                ? $"{cleanFirstName}@{formattedDob}"
                                                                : dto.Password;
              
                            await ProcessOnboardingCoreAsync(dto, evaluatedBulkPassword, actorUserId, cachedMap);
                            result.SuccessfullyProcessedCount++;
                        }
                        catch (Exception ex)
                        {
                            // Intercept and isolate row anomalies (e.g., duplicate registration numbers) 
                            result.Errors.Add(new BulkRowErrorDto
                            {
                                RowNumber = currentSpreadsheetRow,
                                Identifier = $"RegNo: {currentRegNo}",
                                ErrorMessage = ex.InnerException?.Message ?? ex.Message
                            });
                        }
                    }

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception batchBlockException)
                {
                    // If a severe database-level exception occurs (like a lost connection), roll back only this chunk
                    await transaction.RollbackAsync();
                    result.Errors.Add(new BulkRowErrorDto
                    {
                        RowNumber = i + 2,
                        Identifier = $"Batch Chunk Block [{(i / batchSize) + 1}]",
                        ErrorMessage = $"Critical database execution failure within current chunk block: {batchBlockException.Message}"
                    });
                }
            }

            return result;
        }

        public async Task<StudentDto> RegisterSingleStudentAsync(StudentRegistrationDto dto, string actorUserId)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("An explicit initial account credentials password is required for single form registration.");
            }

            var academicMap = await _db.Set<AcademicMap>()
                .Include(m => m.StudyProgram)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.AcademicMapId);

            if (academicMap == null)
            {
                throw new KeyNotFoundException($"Onboarding aborted: The Academic Map configuration profile tracing ID ({dto.AcademicMapId}) does not exist in the system registry.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var studentEntity = await ProcessOnboardingCoreAsync(dto, dto.Password, actorUserId, academicMap);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Maps safely out to your flat standard DTO contract pattern
                return _mapper.Map<StudentDto>(studentEntity);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<Student> ProcessOnboardingCoreAsync(
            StudentRegistrationDto dto,
            string explicitPassword,
            string actorUserId,
            AcademicMap? preLoadedMap = null)
        {
            if (string.IsNullOrWhiteSpace(dto.RegistrationNumber) || dto.RegistrationNumber.Trim().Length != 10)
            {
                throw new ArgumentException("Registration number is invalid. It must be exactly 10 characters long.");
            }

            string cleanRegNo = dto.RegistrationNumber.Trim().ToUpper();

            if (!int.TryParse(cleanRegNo.Substring(0, 2), out int parsedShortYear))
            {
                throw new ArgumentException($"Registration number standard prefix '{cleanRegNo.Substring(0, 2)}' is not a valid year indicator.");
            }

            int startYear = 2000 + parsedShortYear;

            if (preLoadedMap == null || preLoadedMap.StudyProgram == null)
            {
                throw new InvalidOperationException($"The requested configuration academic profile trace is invalid or incomplete.");
            }

            int endYear = startYear + preLoadedMap.StudyProgram.DurationInYears;
            string dynamicBatchTimeline = $"{startYear}-{endYear}";
            string baselineAcademicYear = $"{startYear}-{startYear + 1}";

            string resolvePassword = explicitPassword;
            if (string.IsNullOrWhiteSpace(resolvePassword))
            {
                string cleanFirstName = dto.FirstName?.Replace(" ", "").Trim() ?? "Student";
                string formattedDob = dto.DateOfBirth.ToString("ddMMyyyy");
                resolvePassword = $"{cleanFirstName}@{formattedDob}";
            }

            var coreIdentityAccount = new ApplicationUser
            {
                UserName = cleanRegNo,
                Email = dto.Email.Trim(),
                FirstName = dto.FirstName.Trim(),          
                LastName = dto.LastName.Trim(),            
                IsActive = true,                           
                MustChangePassword = true,                 // Triggers your front-end security router logic on login
                EmailConfirmed = true
            };

            var identityCreationResponse = await _userManager.CreateAsync(coreIdentityAccount, resolvePassword);
            if (!identityCreationResponse.Succeeded)
            {
                string pooledErrors = string.Join(" | ", identityCreationResponse.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Identity Management Guard Blocked Ingestion: {pooledErrors}");
            }

            await _userManager.AddToRoleAsync(coreIdentityAccount, "Student");

            var studentDomainModel = _mapper.Map<Student>(dto);
            studentDomainModel.Guid = Guid.NewGuid();
            studentDomainModel.ApplicationUserId = coreIdentityAccount.Id;
            studentDomainModel.Batch = dynamicBatchTimeline;
            studentDomainModel.AcademicAliasCode = preLoadedMap.AliasCode;
            studentDomainModel.CreatedAt = DateTime.UtcNow;
            studentDomainModel.CreatedBy = actorUserId;

            _db.Set<Student>().Add(studentDomainModel);

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

            _db.Set<StudentAcademicRecord>().Add(baselineSemesterRecord);

            return studentDomainModel;

        }
    }
}
