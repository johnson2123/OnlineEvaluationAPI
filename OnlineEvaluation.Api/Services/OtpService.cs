using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;
using System.Security.Cryptography;

namespace OnlineEvaluation.Api.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _db;

        public OtpService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateAndSaveOtpAsync(string userId, string email, string otpType, string? ipAddress = null, string? deviceInfo = null)
        {
            string otpCode = GenerateSecure6DigitCode();

            var otpLog = new OtpLog
            {
                ApplicationUserId = userId,
                OtpCode = otpCode,
                OtpType = otpType,
                SentTo = email,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5), // OTP valid for 5 minutes
                IsUsed = false,
                AttemptCount = 0,
                IPAddress = ipAddress,
                DeviceInfo = deviceInfo,
                CreatedDate = DateTime.UtcNow
            };

            _db.OtpLogs.Add(otpLog);
            await _db.SaveChangesAsync();

            return otpCode;
        }

        public async Task<bool> VerifyOtpAsync(string userId, string inputOtp, string otpType)
        {
            var latestOtpLog = await _db.OtpLogs
                .Where(o => o.ApplicationUserId == userId && o.OtpType == otpType && !o.IsUsed)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();

            if (latestOtpLog == null)
            {
                return false;
            }

            if (latestOtpLog.AttemptCount >= 5)
            {
                return false;
            }

            latestOtpLog.AttemptCount++;
            await _db.SaveChangesAsync();

            if (DateTime.UtcNow > latestOtpLog.ExpiryTime)
            {
                return false;
            }

            if (latestOtpLog.OtpCode == inputOtp)
            {
                latestOtpLog.IsUsed = true; // Burn the token to prevent reuse attacks
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private static string GenerateSecure6DigitCode()
        {
            int number = RandomNumberGenerator.GetInt32(100000, 1000000);
            return number.ToString();
        }
    }
}
