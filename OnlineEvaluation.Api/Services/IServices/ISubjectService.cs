using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync(int page, int pageSize);
        Task<SubjectDto?> GetByCodeAsync(string code);
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(CreateSubjectDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateSubjectDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
