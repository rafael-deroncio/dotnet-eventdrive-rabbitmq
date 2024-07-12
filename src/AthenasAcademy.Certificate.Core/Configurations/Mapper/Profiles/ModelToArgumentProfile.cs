using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Models;
using AutoMapper;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles;

public class ModelToArgumentProfile : Profile
{
    public ModelToArgumentProfile()
    {
        CreateMap<CertificateModel, CertificateArgument>().ReverseMap();
        CreateMap<DisciplineModel, DisciplineArgument>().ReverseMap();
        CreateMap<FileDetailModel, FileDetailArgument>().ReverseMap();
    }
}