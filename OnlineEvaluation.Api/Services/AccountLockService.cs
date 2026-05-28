using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class AccountLockService : IAccountLockService
    {
        private readonly ApplicationDbContext _db;

        public AccountLockService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> IsAccountLockedAsync(string userId)
        {
            var accountLock = await _db.AccountLocks.FirstOrDefaultAsync(u => u.UserId == userId);

            if (accountLock == null)
            {
                await InitializeAccountSecurityAsync(userId);
   
                return false;
            }

            if (accountLock.IsAccountLocked)
            {
                if (accountLock.LockedUntil.HasValue && accountLock.LockedUntil.Value <= DateTime.UtcNow)
                {
                    accountLock.IsAccountLocked = false;
                    accountLock.LockedUntil = null;
                    accountLock.FailedLoginAttempts = 0;
                    accountLock.UpdatedDate = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                    return false;
                }

                return true;
            }

            return false;

        }
        public async Task<bool> IncrementFailedAttemptsAsync(string userId, int maxAttempts = 5, int lockoutMinutes = 30)
        {
            var accountLock = await _db.AccountLocks.FirstOrDefaultAsync(u => u.UserId == userId);

            if (accountLock == null)
            {
                accountLock = new AccountLock
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                await _db.AccountLocks.AddAsync(accountLock);
            }

            accountLock.FailedLoginAttempts++;
            accountLock.LastFailedLoginDate = DateTime.UtcNow;
            accountLock.UpdatedDate = DateTime.UtcNow;

            bool structuralLockoutTriggered = false;

            if (accountLock.FailedLoginAttempts >= maxAttempts)
            {
                accountLock.IsAccountLocked = true;
                accountLock.LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                structuralLockoutTriggered = true;
            }

            await _db.SaveChangesAsync();
            return structuralLockoutTriggered;
        }

        public async Task InitializeAccountSecurityAsync(string userId)
        {
            var existingRecord = await _db.AccountLocks.AnyAsync(u => u.UserId == userId);
            if (existingRecord) return;

            var newSecurityRow = new AccountLock
            {
                UserId = userId,
                FailedLoginAttempts = 0,
                IsAccountLocked = false,
                SecurityQuestionEnabled = false,
                CreatedDate = DateTime.UtcNow,
                LastPasswordChangedDate = DateTime.UtcNow,
                PasswordExpiryDate = DateTime.UtcNow.AddDays(90)
            };

            await _db.AccountLocks.AddAsync(newSecurityRow);
            await _db.SaveChangesAsync();
        }


        public async Task ResetFailedAttemptsAsync(string userId)
        {
            var accountLock = await _db.AccountLocks.FirstOrDefaultAsync(u => u.UserId == userId);

            if (accountLock != null)
            {
                accountLock.FailedLoginAttempts = 0;
                accountLock.IsAccountLocked = false;
                accountLock.LockedUntil = null;
                accountLock.LastSuccessfulLoginDate = DateTime.UtcNow;
                accountLock.UpdatedDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdatePasswordLifecycleAsync(string userId, int daysUntilExpiry = 90)
        {
            var accountLock = await _db.AccountLocks.FirstOrDefaultAsync(u => u.UserId == userId);
            if (accountLock != null)
            {
                accountLock.LastPasswordChangedDate = DateTime.UtcNow;
                accountLock.PasswordExpiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
                accountLock.UpdatedDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }
        }
    }
}
