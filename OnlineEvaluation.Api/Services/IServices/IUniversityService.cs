using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IUniversityService
    {
        Task<IEnumerable<UniversityDto>> GetAllAsync(int page, int pageSize);
        Task<UniversityDto?> GetByIdAsync(int id);
        Task<UniversityDto?> GetByCodeAsync(string code);
        Task<UniversityDto> CreateAsync(CreateUniversityDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateUniversityDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
