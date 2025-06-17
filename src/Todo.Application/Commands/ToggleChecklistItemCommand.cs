using MediatR;
using System.Text.Json.Serialization;
using ToDo.Domain.DTOs;

namespace Todo.Application.Commands
{
    public class ToggleChecklistItemCommand : IRequest<ChecklistItemDto>
    {
        [JsonIgnore]
        public int TaskId { get; set; }
        public int ItemId { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }
} 