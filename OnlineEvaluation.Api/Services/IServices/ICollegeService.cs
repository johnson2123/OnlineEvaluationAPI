using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ICollegeService
    {
        Task<IEnumerable<CollegeDto>> GetAllAsync(int page, int pageSize);
        Task<CollegeDto?> GetByIdAsync(int id);
        Task<CollegeDto?> GetByCodeAsync(string universityCode, string collegeCode);
        Task<IEnumerable<CollegeDto>> GetByUniversityAsync(string universityCode);
        Task<CollegeDto> CreateAsync(CreateCollegeDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateCollegeDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
