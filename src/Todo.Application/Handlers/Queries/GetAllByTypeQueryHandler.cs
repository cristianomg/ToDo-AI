using AutoMapper;
using MediatR;
using Todo.Application.Queries;
using ToDo.Domain.DTOs;
using ToDo.Domain.Repositories;

namespace Todo.Application.Handlers.Queries
{
    public class GetAllByTypeQueryHandler : IRequestHandler<GetAllByTypeQuery, IEnumerable<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetAllByTypeQueryHandler(
            ITaskRepository taskRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TaskDto>> Handle(GetAllByTypeQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var tasks = await _taskRepository.GetByTypeAndDateAndUserAsync(request.Type, request.Date, request.UserId);

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}
