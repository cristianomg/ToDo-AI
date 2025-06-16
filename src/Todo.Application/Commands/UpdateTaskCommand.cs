using MediatR;
using System.Text.Json.Serialization;
using ToDo.Domain.DTOs;
using ToDo.Domain.Enums;

namespace Todo.Application.Commands
{
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        [JsonIgnore]
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TasksStatus? Status { get; set; }
        public int UserId { get; set; }
    }
}