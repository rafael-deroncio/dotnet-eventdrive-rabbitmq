using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Domain.Requests;
using AutoMapper;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles;

public class EventToRequestProfile : Profile
{
    public EventToRequestProfile()
    {
        CreateMap<CertificateEvent, CertificateRequest>()
        .ForMember(dest => dest.Studant.Name,
                   opts => opts.MapFrom(src => src.StudantName))
        .ForMember(dest => dest.Studant.BornDate,
                   opts => opts.MapFrom(src => src.StudantBornDate))
        .ForMember(dest => dest.Studant.Registration,
                   opts => opts.MapFrom(src => src.Registration))

        .ForMember(dest => dest.Studant.Document.Number,
                   opts => opts.MapFrom(src => src.DocumentNumber))
        .ForMember(dest => dest.Studant.Document.Type,
                   opts => opts.MapFrom(src => src.DocumentType))
                   
        .ForMember(dest => dest.Course.Name,
                   opts => opts.MapFrom(src => src.CourseName))
        .ForMember(dest => dest.Course.Workload,
                   opts => opts.MapFrom(src => src.CourseWorkload))
        .ReverseMap();
    }
}
