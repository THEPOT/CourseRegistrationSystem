using AutoMapper;
using CDQTSystem_Domain.Entities;
using CDQTSystem_API.Payload.Response;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentResponse>();
        // Add other mappings as needed
    }
}