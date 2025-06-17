using AutoMapper;
using ToDo.Domain.DTOs;
using ToDo.Domain.Entities;

namespace Todo.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Tasks, TaskDto>();
        CreateMap<ChecklistItem, ChecklistItemDto>();
    }
} 