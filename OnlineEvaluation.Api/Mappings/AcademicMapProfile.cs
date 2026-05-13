using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class AcademicMapProfile : Profile
    {
        public AcademicMapProfile()
        {
   
            // We "flatten" the navigation properties into simple strings for the React table
            CreateMap<AcademicMap, AcademicMapDto>()
                .ForMember(dest => dest.CollegeName, opt => opt.MapFrom(src => src.College.Name))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.StudyProgram.Name))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name));


            CreateMap<CreateAcademicMapDto, AcademicMap>()
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));


            CreateMap<UpdateAcademicMapDto, AcademicMap>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // 4. LookUp Mappings 
            CreateMap<College, LookUpDto>();
            CreateMap<StudyProgram, LookUpDto>();
            CreateMap<Branch, LookUpDto>();
        }
    }
}
