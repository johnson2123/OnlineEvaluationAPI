using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            ValueTransformers.Add<string>(val => val == null ? null : val.Trim());

            CreateMap<StaffRegistrationDto, StaffProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StaffGuid, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())

                .ForMember(dest => dest.CollegeDepartment, opt => opt.Ignore())
                .ForMember(dest => dest.CollegeDepartmentAliasCode, opt => opt.Ignore())
                .ForMember(dest => dest.ReportsToProfile, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())

                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))

                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.EmployeeId) ? src.EmployeeId.Trim().ToUpper() : string.Empty))

                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HighestQualification, opt => opt.MapFrom(src => src.HighestQualification))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CollegeDepartmentId, opt => opt.MapFrom(src => src.CollegeDepartmentId))
                .ForMember(dest => dest.ReportsToProfileId, opt => opt.MapFrom(src => src.ReportsToProfileId))
                .ForMember(dest => dest.IsTeachingStaff, opt => opt.MapFrom(src => src.IsTeachingStaff))
                .ForMember(dest => dest.IsPermanent, opt => opt.MapFrom(src => src.IsPermanent));


            CreateMap<StaffProfile, StaffDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StaffGuid, opt => opt.MapFrom(src => src.StaffGuid))
                // Safe cross-table security context data extractions
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                    src.ApplicationUser != null ? src.ApplicationUser.Email : string.Empty))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
                    src.ApplicationUser != null ? src.ApplicationUser.FirstName : string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
                    src.ApplicationUser != null ? src.ApplicationUser.LastName : string.Empty))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))

                .ForMember(dest => dest.CollegeDepartmentAliasCode, opt => opt.MapFrom(src => src.CollegeDepartmentAliasCode))
                .ForMember(dest => dest.CollegeDepartmentName, opt => opt.MapFrom(src =>
                    src.CollegeDepartment != null && src.CollegeDepartment.Department != null
                        ? src.CollegeDepartment.Department.Name
                        : string.Empty))

                .ForMember(dest => dest.ReportsToProfileId, opt => opt.MapFrom(src => src.ReportsToProfileId))
                .ForMember(dest => dest.ReportsToStaffName, opt => opt.MapFrom(src =>
                    src.ReportsToProfile != null ? src.ReportsToProfile.FullName : null));
        }
    }
}