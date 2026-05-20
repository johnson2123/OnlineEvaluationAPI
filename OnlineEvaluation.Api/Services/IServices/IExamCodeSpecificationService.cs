using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IExamCodeSpecificationService
    {
        Task<IEnumerable<ExamSpecDto>> GetAllAsync(int page, int pageSize);
        Task<ExamSpecDto?> GetByIdAsync(int id);
        Task<ExamSpecDto> CreateAsync(CreateExamSpecDto dto, string? actorUserId);
        Task<bool> UpdateAsync(int id, UpdateExamSpecDto dto, string? actorUserId);
        Task<bool> SoftDeleteAsync(int id, string? actorUserId);
        Task<bool> HardDeleteAsync(int id);
    }
}
