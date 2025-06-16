using MediatR;
using ToDo.Domain.DTOs;

namespace Todo.Application.Queries
{
    public class GetTaskByIdQuery : IRequest<TaskDto>
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }
}
