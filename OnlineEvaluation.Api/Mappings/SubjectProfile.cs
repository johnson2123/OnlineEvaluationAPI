using AutoMapper;
using OnlineEvaluation.Api.Constants;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {


            CreateMap<Subject, SubjectDto>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()));


            CreateMap<CreateSubjectDto, Subject>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateSubjectDto, Subject>()
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }

    }
}
