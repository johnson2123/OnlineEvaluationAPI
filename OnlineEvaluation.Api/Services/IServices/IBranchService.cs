using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IBranchService
    {
        
        Task<IEnumerable<BranchDto>> GetAllAsync(int page, int pageSize);
        Task<BranchDto?> GetByCodeAsync(string code);
        Task<BranchDto?> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(CreateBranchDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateBranchDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
