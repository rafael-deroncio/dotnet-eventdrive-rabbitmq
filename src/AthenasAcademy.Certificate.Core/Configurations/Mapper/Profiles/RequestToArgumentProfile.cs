using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Domain.Requests;
using AutoMapper;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles;

public class RequestToArgumentProfile : Profile
{
    public RequestToArgumentProfile()
    {
        CreateMap<CertificateRequest, CertificateArgument>().ReverseMap();
        CreateMap<DisciplineRequest, DisciplineArgument>().ReverseMap();
    }
}