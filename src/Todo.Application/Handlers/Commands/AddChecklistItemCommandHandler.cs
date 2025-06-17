using AutoMapper;
using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.DTOs;
using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, ChecklistItemDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public AddChecklistItemCommandHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<ChecklistItemDto> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {request.TaskId} not found");
            }

            if (task.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only modify your own tasks");
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new ArgumentException("Checklist item text cannot be empty");
            }

            var checklistItem = new ChecklistItem(request.Text, request.TaskId);
            task.Checklist.Add(checklistItem);

            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<ChecklistItemDto>(checklistItem);
        }
    }
} 