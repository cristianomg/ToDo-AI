using MediatR;
using System.Text.Json.Serialization;
using ToDo.Domain.DTOs;

namespace Todo.Application.Commands
{
    public class AddChecklistItemCommand : IRequest<ChecklistItemDto>
    {
        [JsonIgnore]
        public int TaskId { get; set; }
        public string Text { get; set; } = string.Empty;
        [JsonIgnore]
        public int UserId { get; set; }
    }
} 