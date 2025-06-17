using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class DeleteChecklistItemCommandHandler : IRequestHandler<DeleteChecklistItemCommand, bool>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteChecklistItemCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<bool> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId, x=>x.Checklist);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {request.TaskId} not found");
            }

            if (task.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only modify your own tasks");
            }

            var checklistItem = task.Checklist.FirstOrDefault(c => c.Id == request.ItemId);
            if (checklistItem == null)
            {
                throw new KeyNotFoundException($"Checklist item with ID {request.ItemId} not found");
            }

            task.Checklist.Remove(checklistItem);

            await _taskRepository.SaveChangesAsync();

            return true;
        }
    }
} 