using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class UniversityProfile : Profile
    {
        public UniversityProfile()
        {
            CreateMap<University, UniversityDto>();

            CreateMap<CreateUniversityDto, University>()
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByUserId, opt => opt.Ignore());

            CreateMap<UpdateUniversityDto, University>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
