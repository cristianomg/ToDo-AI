using MediatR;
using System.Text.Json.Serialization;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Domain.DTOs;

namespace Todo.Application.Commands
{
    public class CreateTaskCommand : IRequest<List<Tasks>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartAt { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskType Type { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public List<ChecklistItemDto>? Checklist { get; set; }
    }
} 