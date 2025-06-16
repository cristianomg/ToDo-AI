using MediatR;
using System.Text.Json.Serialization;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;

namespace Todo.Application.Commands
{
    public class CreateTaskCommand : IRequest<Tasks>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskType Type { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }
} 