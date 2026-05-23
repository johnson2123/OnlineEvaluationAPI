using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IStaffOnboardingService
    {
        Task<StaffDto> RegisterSingleStaffAsync(StaffRegistrationDto dto, string actorUserId);
        Task<BulkOperationResultDto<BulkRowErrorDto>> RegisterBulkStaffAsync(List<StaffRegistrationDto> dtos, string actorUserId);
    }
}
