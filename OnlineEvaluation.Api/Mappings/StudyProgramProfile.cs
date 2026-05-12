using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class StudyProgramProfile : Profile
    {
        public StudyProgramProfile()
        {
            CreateMap<StudyProgram, StudyProgramDto>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()));

            CreateMap<CreateStudyProgramDto, StudyProgram>();
            CreateMap<UpdateStudyProgramDto, StudyProgram>();
        }
    }
}
