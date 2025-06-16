using MediatR;

namespace Todo.Application.Commands
{
    public class DeleteTaskCommand : IRequest
    {
        public int UserId { get; set; }
        public int TaskId { get; set; }
    }
}
