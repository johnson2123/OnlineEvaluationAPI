using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IStudentOnboardingService
    {
        Task<StudentDto> RegisterSingleStudentAsync(StudentRegistrationDto dto, string actorUserId);
        Task<BulkOperationResultDto<BulkRowErrorDto>> RegisterBulkStudentsAsync(List<StudentRegistrationDto> dtos, string actorUserId);
    }
}
