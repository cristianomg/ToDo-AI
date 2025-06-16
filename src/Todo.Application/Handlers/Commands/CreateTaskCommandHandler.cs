using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Infrastructure.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Tasks>
    {
        private readonly BaseRepository<Tasks> _taskRepository;

        public CreateTaskCommandHandler(BaseRepository<Tasks> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Tasks> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var dueDate = CalculateDueDate(request.Type);

            var task = new Tasks(
                request.Title,
                request.Description,
                dueDate,
                request.Priority,
                request.Type,
                request.UserId
            );

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return task;
        }

        private DateTime CalculateDueDate(TaskType type)
        {
            var now = DateTime.UtcNow;

            return DateTime.SpecifyKind(type switch
            {
                TaskType.Daily => new DateTime(now.Year, now.Month, now.Day, 23, 59, 59),
                
                TaskType.Weekly => GetEndOfWeek(now),
                
                TaskType.Monthly => new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59),
                
                _ => throw new ArgumentException("Invalid task type", nameof(type))
            }, DateTimeKind.Utc);
        }

        private DateTime GetEndOfWeek(DateTime date)
        {
            // Get the current day of week (0 = Sunday, 6 = Saturday)
            var currentDayOfWeek = (int)date.DayOfWeek;
            
            // Calculate days until Sunday (end of week)
            var daysUntilEndOfWeek = 7 - currentDayOfWeek;
            
            // Add days to get to Sunday
            var endOfWeek = date.AddDays(daysUntilEndOfWeek);
            
            // Set time to end of day
            return new DateTime(endOfWeek.Year, endOfWeek.Month, endOfWeek.Day, 23, 59, 59);
        }
    }
} 