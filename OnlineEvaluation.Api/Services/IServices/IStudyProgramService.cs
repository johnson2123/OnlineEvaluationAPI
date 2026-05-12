using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IStudyProgramService
    {
        Task<IEnumerable<StudyProgramDto>> GetAllAsync(int page, int pageSize);
        Task<StudyProgramDto?> GetByIdAsync(int id);
        Task<StudyProgramDto> CreateAsync(CreateStudyProgramDto createDto, string userId);
        Task<bool> UpdateAsync(UpdateStudyProgramDto updateDto, string userId);
        Task<bool> SoftDeleteAsync(int id, string userId);
        Task<bool> HardDeleteAsync(int id);
        Task<bool> RestoreAsync(int id); 
    }
}
