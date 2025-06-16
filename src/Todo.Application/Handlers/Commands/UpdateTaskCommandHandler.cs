using AutoMapper;
using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.DTOs;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskCommandHandler(
            ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {request.TaskId} not found");
            }

            if (task.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only update your own tasks");
            }

            // Update task properties if they are provided
            if (request.Title != null)
            {
                task.UpdateTitle(request.Title);
            }

            if (request.Description != null)
            {
                task.UpdateDescription(request.Description);
            }

            if (request.Status.HasValue)
            {
                task.UpdateStatus(request.Status.Value);
            }

            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }
    }
}