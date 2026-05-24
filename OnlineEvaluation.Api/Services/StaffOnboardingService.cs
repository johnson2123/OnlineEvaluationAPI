using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class StaffOnboardingService : IStaffOnboardingService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffOnboardingService> _logger;
        IMfaSecurityService _mfaSecurity;

        public StaffOnboardingService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<StaffOnboardingService> logger,
            IMfaSecurityService mfaSecurity)
        {
            _db = db;
            _userManager = userManager;
            _mfaSecurity = mfaSecurity;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StaffDto> RegisterSingleStaffAsync(StaffRegistrationDto dto, string actorUserId)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("An explicit initial account credentials password is required for single form registration.");
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ArgumentException("Onboarding rejected: First name is required and cannot be empty.");
            }

            var departmentNode = await _db.Set<CollegeDepartment>()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.CollegeDepartmentId);

            if (departmentNode == null)
            {
                throw new KeyNotFoundException($"Onboarding aborted: The College Department reference identity ({dto.CollegeDepartmentId}) does not exist in the system registry.");
            }

            ApplicationUser createdIdentity = null;
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var resultTuple = await ProcessOnboardingCoreAsync(dto, dto.Password, actorUserId, departmentNode);
                createdIdentity = resultTuple.IdentityUser;
                var staffEntity = resultTuple.Profile;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var completeRecord = await _db.Set<StaffProfile>()
                    .Include(s => s.ApplicationUser)
                    .Include(s => s.CollegeDepartment)
                        .ThenInclude(cd => cd.Department)
                    .Include(s => s.ReportsToProfile)
                    .FirstAsync(s => s.Id == staffEntity.Id);

                return _mapper.Map<StaffDto>(completeRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Single staff onboarding failed for Employee ID: {EmployeeId}", dto.EmployeeId);
                await transaction.RollbackAsync();

                if (createdIdentity != null)
                {
                    await _userManager.DeleteAsync(createdIdentity);
                }
                throw;
            }
        }

        public async Task<BulkOperationResultDto<BulkRowErrorDto>> RegisterBulkStaffAsync(List<StaffRegistrationDto> dtos, string actorUserId)
        {
            var result = new BulkOperationResultDto<BulkRowErrorDto>
            {
                TotalRecordsReceived = dtos?.Count ?? 0,
                Errors = new List<BulkRowErrorDto>()
            };

            if (dtos == null || !dtos.Any())
                return result;

            var departmentsMatrix = await _db.Set<CollegeDepartment>()
                .AsNoTracking()
                .ToDictionaryAsync(d => d.Id);

            const int batchSize = 100;

            for (int i = 0; i < dtos.Count; i += batchSize)
            {
                var currentBatchChunk = dtos.Skip(i).Take(batchSize).ToList();

                var empIds = currentBatchChunk.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeId)).Select(x => x.EmployeeId.Trim().ToUpper()).ToList();
                var emails = currentBatchChunk.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email.Trim().ToLower()).ToList();

                var existingUsers = await _db.Set<ApplicationUser>()
                    .Where(u => empIds.Contains(u.UserName) || emails.Contains(u.Email))
                    .Select(u => new { u.UserName, u.Email })
                    .AsNoTracking()
                    .ToListAsync();

                var existingEmpIds = existingUsers.Select(u => u.UserName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var existingEmails = existingUsers.Select(u => u.Email).ToHashSet(StringComparer.OrdinalIgnoreCase);

                using var chunkTransaction = await _db.Database.BeginTransactionAsync();
                var transientIdentitiesInChunk = new List<ApplicationUser>();
                bool chunkAborted = false;

                for (int j = 0; j < currentBatchChunk.Count; j++)
                {
                    var dto = currentBatchChunk[j];
                    int currentSpreadsheetRow = i + j + 2;
                    string currentEmpId = dto.EmployeeId?.Trim().ToUpper() ?? "UNKNOWN";
                    string currentEmail = dto.Email?.Trim().ToLower() ?? "UNKNOWN";

                    if (dto.ReportsToProfileId == 0)
                    {
                        dto.ReportsToProfileId = null;
                    }

                    if (string.IsNullOrWhiteSpace(dto.FirstName))
                    {
                        result.Errors.Add(new BulkRowErrorDto
                        {
                            RowNumber = currentSpreadsheetRow,
                            Identifier = $"EmpId: {currentEmpId}",
                            ErrorMessage = "Row skipped: Staff record does not contain a valid First Name."
                        });
                        continue;
                    }

                    if (!departmentsMatrix.TryGetValue(dto.CollegeDepartmentId, out var cachedDept))
                    {
                        result.Errors.Add(new BulkRowErrorDto
                        {
                            RowNumber = currentSpreadsheetRow,
                            Identifier = $"EmpId: {currentEmpId}",
                            ErrorMessage = $"CollegeDepartment reference identity ({dto.CollegeDepartmentId}) was not found in the registry."
                        });
                        continue;
                    }

                    if (existingEmpIds.Contains(currentEmpId) || existingEmails.Contains(currentEmail))
                    {
                        result.Errors.Add(new BulkRowErrorDto
                        {
                            RowNumber = currentSpreadsheetRow,
                            Identifier = $"EmpId: {currentEmpId}",
                            ErrorMessage = "An account record with matching identifier keys (Employee ID or Email Address) already exists."
                        });
                        continue;
                    }

                    try
                    {
                        string cleanFirstName = dto.FirstName?.Replace(" ", "").Trim() ?? "Staff";
                        string evaluatedBulkPassword = string.IsNullOrWhiteSpace(dto.Password)
                            ? $"{cleanFirstName}@{currentEmpId}"
                            : dto.Password;

                        var coreResult = await ProcessOnboardingCoreAsync(dto, evaluatedBulkPassword, actorUserId, cachedDept);
                        transientIdentitiesInChunk.Add(coreResult.IdentityUser);
                    }
                    catch (Exception ex)
                    {
                        chunkAborted = true;
                        _logger.LogError(ex, "Chunk block failure triggered structural rollback at position context {Row}", currentSpreadsheetRow);

                        for (int k = 0; k < currentBatchChunk.Count; k++)
                        {
                            var fallbackDto = currentBatchChunk[k];
                            result.Errors.Add(new BulkRowErrorDto
                            {
                                RowNumber = i + k + 2,
                                Identifier = $"EmpId: {fallbackDto.EmployeeId?.Trim().ToUpper() ?? "UNKNOWN"}",
                                ErrorMessage = $"Chunk block execution aborted due to a failure on row {currentSpreadsheetRow}: {ex.InnerException?.Message ?? ex.Message}"
                            });
                        }
                        break;
                    }
                }

                if (chunkAborted)
                {
                    await chunkTransaction.RollbackAsync();

                    foreach (var identity in transientIdentitiesInChunk)
                    {
                        await _userManager.DeleteAsync(identity);
                    }

                    _db.ChangeTracker.Clear();
                }
                else
                {
                    try
                    {
                        await _db.SaveChangesAsync();
                        await chunkTransaction.CommitAsync();

                        result.SuccessfullyProcessedCount += transientIdentitiesInChunk.Count;
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(dbEx, "Database transaction commit crash for chunk group starting at index {StartIndex}", i);

                        await chunkTransaction.RollbackAsync();

                        foreach (var identity in transientIdentitiesInChunk)
                        {
                            await _userManager.DeleteAsync(identity);
                        }

                        _db.ChangeTracker.Clear();

                        for (int k = 0; k < currentBatchChunk.Count; k++)
                        {
                            var fallbackDto = currentBatchChunk[k];
                            result.Errors.Add(new BulkRowErrorDto
                            {
                                RowNumber = i + k + 2,
                                Identifier = $"EmpId: {fallbackDto.EmployeeId?.Trim().ToUpper() ?? "UNKNOWN"}",
                                ErrorMessage = $"Database transaction commit crash for chunk group: {dbEx.InnerException?.Message ?? dbEx.Message}"
                            });
                        }
                    }
                }
            }

            return result;
        }

        private async Task<(ApplicationUser IdentityUser, StaffProfile Profile)> ProcessOnboardingCoreAsync(
            StaffRegistrationDto dto,
            string explicitPassword,
            string actorUserId,
            CollegeDepartment preLoadedDept)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                throw new ArgumentException("Employee Identification tracking number cannot be empty.");
            }

            string cleanEmpId = dto.EmployeeId.Trim().ToUpper();

            var coreIdentityAccount = new ApplicationUser
            {
                UserName = cleanEmpId,
                Email = dto.Email?.Trim(),
                FirstName = dto.FirstName?.Trim(),
                LastName = dto.LastName?.Trim(),
                IsActive = true,
                MustChangePassword = true,
                EmailConfirmed = true
            };

            var identityCreationResponse = await _userManager.CreateAsync(coreIdentityAccount, explicitPassword);
            if (!identityCreationResponse.Succeeded)
            {
                string pooledErrors = string.Join(" | ", identityCreationResponse.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Identity Management Guard Blocked Ingestion: {pooledErrors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(coreIdentityAccount, dto.Role.Trim());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(coreIdentityAccount);
                string roleErrors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
            }

            var mfaSetting = new UserMFASetting
            {
                ApplicationUserId = coreIdentityAccount.Id,
                IsMFAEnabled = dto.IsMfaEnabled,
                MFAType = dto.MFAType,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.IsMfaEnabled && dto.MFAType == "AuthenticatorApp")
            {
                mfaSetting.SecretKey = string.IsNullOrWhiteSpace(dto.SecretKey)
                    ? _mfaSecurity.GenerateRandomSecretKey()
                    : dto.SecretKey;

                var backupCodesList = _mfaSecurity.GenerateBackupCodes();
                mfaSetting.BackupCodes = string.Join(",", backupCodesList);
            }

            await _db.Set<UserMFASetting>().AddAsync(mfaSetting);

            var staffDomainModel = _mapper.Map<StaffProfile>(dto);
            staffDomainModel.StaffGuid = Guid.NewGuid();
            staffDomainModel.ApplicationUserId = coreIdentityAccount.Id;
            staffDomainModel.CollegeDepartmentAliasCode = preLoadedDept.AliasCode;
            staffDomainModel.CreatedAt = DateTime.UtcNow;
            staffDomainModel.CreatedBy = actorUserId;

            _db.Set<StaffProfile>().Add(staffDomainModel);

            return (coreIdentityAccount, staffDomainModel);
        }
    }
}