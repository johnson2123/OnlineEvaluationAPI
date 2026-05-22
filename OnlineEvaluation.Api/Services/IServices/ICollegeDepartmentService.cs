using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ICollegeDepartmentService
    {
        Task<IEnumerable<CollegeDepartmentDto>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<CollegeDepartmentDto>> GetByCollegeAsync(int collegeId, int page, int pageSize);
        Task<CollegeDepartmentDto?> GetByIdAsync(int id);
        Task<CollegeDepartmentDto> MapDepartmentAsync(CreateCollegeDepartmentDto dto, string userId);
        Task<bool> UpdateStatusAsync(int id, bool isActive, string userId);
        Task<bool> RemoveMappingAsync(int id, string userId, bool hardDelete = false);
    }
}
