using AutoMapper;
using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.DTOs;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class ToggleChecklistItemCommandHandler : IRequestHandler<ToggleChecklistItemCommand, ChecklistItemDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public ToggleChecklistItemCommandHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<ChecklistItemDto> Handle(ToggleChecklistItemCommand request, CancellationToken cancellationToken)
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

            checklistItem.Completed = !checklistItem.Completed;

            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<ChecklistItemDto>(checklistItem);
        }
    }
} 