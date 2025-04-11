using AutoMapper;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Entities;

namespace CDQTSystem_API.Mapper
{
    public class SemesterMapper : Profile
    {
        public SemesterMapper()
        {
            CreateMap<Semester, SemesterResponse>();
        }
    }
}