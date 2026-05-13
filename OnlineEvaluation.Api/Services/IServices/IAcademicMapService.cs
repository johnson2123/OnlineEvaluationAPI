using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IAcademicMapService
    {
        Task<AcademicMapInitDto> GetInitDataAsync();
        Task<IEnumerable<AcademicMapDto>> GetAllAsync(int page, int pageSize);
        Task<AcademicMapDto?> GetByIdAsync(int id);
        Task<AcademicMapDto> CreateAsync(CreateAcademicMapDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateAcademicMapDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
