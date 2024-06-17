using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Domain.Responses;
using AutoMapper;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles;

public class ModelToResponseProfile : Profile
{
    public ModelToResponseProfile()
    {
        CreateMap<CertificateModel, CertificateResponse>()
        .ForMember(dest => dest.Details.Studant, 
            opts => opts.MapFrom(src => src.StudentName))
        .ForMember(dest => dest.Details.Course, 
            opts => opts.MapFrom(src => src.Course))
        .ForMember(dest => dest.Details.Workload, 
            opts => opts.MapFrom(src => src.Workload))
        .ForMember(dest => dest.Details.Utilization, 
            opts => opts.MapFrom(src => src.Utilization))
        .ForMember(dest => dest.Details.Completion, 
            opts => opts.MapFrom(src => src.Completion))        
        .ReverseMap();
    }
}