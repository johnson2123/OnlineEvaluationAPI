using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class LoginAuditService : ILoginAuditService
    {
        private readonly ApplicationDbContext _db;

        public LoginAuditService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task LogLoginAttemptAsync(string userId, string status, string? failureReason = null, string? ipAddress = null, string? browserInfo = null, string? osInfo = null, string? deviceInfo = null, string? sessionId = null)
        {
            var auditEntry = new LoginAudit
            {
                UserId = userId,
                LoginStatus = status, 
                FailureReason = failureReason,
                IPAddress = ipAddress,
                BrowserInfo = browserInfo,
                OperatingSystem = osInfo,
                DeviceInfo = deviceInfo,
                SessionId = sessionId,
                LoginTime = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            await _db.LoginAudits.AddAsync(auditEntry);
            await _db.SaveChangesAsync();

        }

        public async Task LogLogoutAsync(string userId, string sessionId)
        {
            var activeSession = await _db.LoginAudits
                .Where(u => u.UserId == userId && u.SessionId == sessionId && u.LoginStatus == "Success")
                .OrderByDescending(u => u.LoginTime)
                .FirstOrDefaultAsync();

            if (activeSession != null)
            {
                activeSession.LogoutTime = DateTime.UtcNow;
             
                var logoutRecord = new LoginAudit
                {
                    UserId = userId,
                    LoginStatus = "Logout",
                    SessionId = sessionId,
                    LoginTime = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                await _db.LoginAudits.AddAsync(logoutRecord);
                await _db.SaveChangesAsync();
            }
        }
    }
}
