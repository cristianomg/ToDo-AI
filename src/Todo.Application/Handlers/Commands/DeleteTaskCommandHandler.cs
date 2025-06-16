using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {request.TaskId} not found");

            if (task.UserId != request.UserId)
                throw new UnauthorizedAccessException($"User {request.UserId} is not authorized to delete task {request.TaskId}");

            await _taskRepository.DeleteByIdAsync(request.TaskId);
            await _taskRepository.SaveChangesAsync();
        }
    }
}
