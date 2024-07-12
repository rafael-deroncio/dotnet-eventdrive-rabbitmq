using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Domain.Responses;
using AutoMapper;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles;

public class ModelToResponseProfile : Profile
{
    public ModelToResponseProfile()
    {
        CreateMap<CertificateModel, CertificateResponse>()
        .ForMember(dest => dest.Student, 
            opts => opts.MapFrom(src => src.StudentName))
        .ForMember(dest => dest.Course, 
            opts => opts.MapFrom(src => src.Course))
        .ForMember(dest => dest.Workload, 
            opts => opts.MapFrom(src => src.Workload))
        .ForMember(dest => dest.Utilization, 
            opts => opts.MapFrom(src => src.Utilization))
        .ForMember(dest => dest.Conclusion, 
            opts => opts.MapFrom(src => src.Conclusion))
        .ReverseMap();

        CreateMap<FileDetailModel, FileResponse>()
        .ForMember(dest => dest.Name, 
            opts => opts.MapFrom(src => Path.GetFileName(src.File)))
        .ForMember(dest => dest.Size, 
            opts => opts.MapFrom(src => src.Size))
        .ForMember(dest => dest.Type, 
            opts => opts.MapFrom(src => src.Type))
        .ReverseMap();
    }
}