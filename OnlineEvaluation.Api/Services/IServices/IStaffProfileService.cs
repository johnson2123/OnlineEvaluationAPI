using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IStaffProfileService
    {
        Task<StaffProfileDto?> GetProfileByUserIdAsync(string applicationUserId);
        Task<bool> UpdateProfileAsync(string applicationUserId, UpdateStaffProfileDto dto);
    }
}
