using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class StudentProfileProfile : Profile
    {
        public StudentProfileProfile()
        {
            CreateMap<Student, StudentProfileDto>()
                .ForMember(dest => dest.ProfileGuid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                    $"{src.User.FirstName} {src.User.LastName}".Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.AcademicMap.Branch.Name))
                .ForMember(dest => dest.Regulation, opt => opt.MapFrom(src => src.AcademicMap.Regulation));

        }
    }
}
