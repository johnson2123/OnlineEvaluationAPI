using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IStudentProfileService
    {
        Task<StudentProfileDto?> GetProfileByUserIdAsync(string applicationUserId);
        Task<bool> UpdateProfileAsync(string applicationUserId, UpdateStudentProfileDto dto);
    }
}
