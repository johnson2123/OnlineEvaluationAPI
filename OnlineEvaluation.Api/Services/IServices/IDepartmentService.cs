using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync(int page, int pageSize);
        Task<DepartmentDto?> GetByCodeAsync(string code);
        Task<DepartmentDto?> GetByIdAsync(int id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
