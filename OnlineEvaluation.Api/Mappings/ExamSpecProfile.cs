using AutoMapper;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Mappings
{
    public class ExamSpecProfile : Profile
    {
        public ExamSpecProfile()
        {
            CreateMap<ExamCodeSpecification, ExamSpecDto>()
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.Code))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.IsElective, opt => opt.MapFrom(src => src.Subject.IsElective))
                .ForMember(dest => dest.Regulation, opt => opt.MapFrom(src => src.AcademicMap.Regulation));

            CreateMap<CreateExamSpecDto, ExamCodeSpecification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.ExamSpecCode, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

            CreateMap<UpdateExamSpecDto, ExamCodeSpecification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Guid, opt => opt.Ignore())
                .ForMember(dest => dest.ExamSpecCode, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicMapId, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
        }
    }
}
