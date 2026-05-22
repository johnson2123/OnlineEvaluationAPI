using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class CollegeDepartmentProfile : Profile
    {
        public CollegeDepartmentProfile()
        {
            CreateMap<CollegeDepartment, CollegeDepartmentDto>()
                .ForMember(dest => dest.CollegeName, opt => opt.MapFrom(src => src.College.Name))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.Department.Code));
        }
    }
}
