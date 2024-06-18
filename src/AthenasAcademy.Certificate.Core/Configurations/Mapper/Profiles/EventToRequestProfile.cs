using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Domain.Requests;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace AthenasAcademy.Certificate.Core.Configurations.Mapper.Profiles
{
    public class EventToRequestProfile : Profile
    {
        public EventToRequestProfile()
        {
            CreateMap<CertificateEvent, CertificateRequest>()
                .ForPath(dest => dest.Studant.Name,
                    opts => opts.MapFrom(src => src.StudantName))
                .ForPath(dest => dest.Studant.BornDate,
                    opts => opts.MapFrom(src => src.StudantBornDate))
                .ForPath(dest => dest.Studant.Registration,
                    opts => opts.MapFrom(src => src.Registration))
                .ForPath(dest => dest.Studant.Document.Number,
                    opts => opts.MapFrom(src => src.DocumentNumber))
                .ForPath(dest => dest.Studant.Document.Type,
                    opts => opts.MapFrom(src => src.DocumentType))
                .ForPath(dest => dest.Course.Name,
                    opts => opts.MapFrom(src => src.CourseName))
                .ForPath(dest => dest.Course.Workload,
                    opts => opts.MapFrom(src => src.CourseWorkload))
                .ForPath(dest => dest.Course.Subjects,
                    opts => opts.MapFrom(src => new DictionaryToListTypeConverter().Convert(src.CourseSubjects, null)))
                .ReverseMap()
                .ForPath(dest => dest.CourseSubjects,
                    opts => opts.MapFrom(src => new ListToDictionaryTypeConverter().Convert(src.Course.Subjects, null)));
        }
    }

    public class DictionaryToListTypeConverter : IValueConverter<Dictionary<string, int>, List<DisciplineRequest>>
    {
        public List<DisciplineRequest> Convert(Dictionary<string, int> source, ResolutionContext context)
        {
            return source.Select(kvp => new DisciplineRequest { Name = kvp.Key, Workload = kvp.Value }).ToList();
        }
    }

    public class ListToDictionaryTypeConverter : IValueConverter<List<DisciplineRequest>, Dictionary<string, int>>
    {
        public Dictionary<string, int> Convert(List<DisciplineRequest> source, ResolutionContext context)
        {
            return source.ToDictionary(d => d.Name, d => d.Workload);
        }
    }
}