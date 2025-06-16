using MediatR;
using ToDo.Domain.DTOs;
using ToDo.Domain.Enums;

namespace Todo.Application.Queries
{
    public class GetAllByTypeQuery : IRequest<IEnumerable<TaskDto>>
    {
        public DateTime Date { get; set; }
        public TaskType Type { get; set; }
        public int UserId { get; set; }
    }
}
