using AutoMapper;
using MediatR;
using Todo.Application.Queries;
using ToDo.Domain.DTOs;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Queries
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTaskByIdQueryHandler(
            ITaskRepository taskRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var task = await _taskRepository.GetByIdWithDetailsAsync(request.TaskId);
            
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {request.TaskId} not found");

            if (task.UserId != request.UserId)
                throw new UnauthorizedAccessException($"User {request.UserId} is not authorized to access task {request.TaskId}");

            return _mapper.Map<TaskDto>(task);
        }
    }
}