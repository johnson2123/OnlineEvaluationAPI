using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<StudentRegistrationDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Batch, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicAliasCode, opt => opt.Ignore())

                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicMap, opt => opt.Ignore())

                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))

                // Robust String Normalization & Clean Trimming Configurations
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.RegistrationNumber) ? src.RegistrationNumber.Trim().ToUpper() : null))
                .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.FatherName) ? src.FatherName.Trim() : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Gender) ? src.Gender.Trim() : null))
                .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.ContactNumber) ? src.ContactNumber.Trim() : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Address) ? src.Address.Trim() : null))
                .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.BloodGroup) ? src.BloodGroup.Trim().ToUpper() : null));

             CreateMap<Student, StudentDto>();
        }
    }
}
