using MediatR;
using Todo.Application.Commands;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Infrastructure.Repositories;

namespace Todo.Application.Handlers.Commands
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, List<Tasks>>
    {
        private readonly BaseRepository<Tasks> _taskRepository;

        public CreateTaskCommandHandler(BaseRepository<Tasks> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<Tasks>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var tasks = new List<Tasks>();

            if (!request.IsRecurring || !request.RecurrenceEndDate.HasValue)
            {
                // Criar apenas uma tarefa
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
                tasks.Add(task);
            }
            else
            {
                // Criar múltiplas tarefas recorrentes
                var recurringTasks = GenerateRecurringTasks(request);
                
                foreach (var task in recurringTasks)
                {
                    await _taskRepository.AddAsync(task);
                    tasks.Add(task);
                }
            }

            await _taskRepository.SaveChangesAsync();
            return tasks;
        }

        private List<Tasks> GenerateRecurringTasks(CreateTaskCommand request)
        {
            var tasks = new List<Tasks>();
            var startDate = DateTime.UtcNow.Date;
            var endDate = request.RecurrenceEndDate.Value.Date;
            
            var currentDate = startDate;

            switch (request.Type)
            {
                case TaskType.Daily:
                    while (currentDate <= endDate)
                    {
                        var dueDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59, DateTimeKind.Utc);
                        tasks.Add(new Tasks(
                            request.Title,
                            request.Description,
                            dueDate,
                            request.Priority,
                            request.Type,
                            request.UserId
                        ));
                        currentDate = currentDate.AddDays(1);
                    }
                    break;

                case TaskType.Weekly:
                    // Ajustar para o início da semana (segunda-feira)
                    var dayOfWeek = (int)currentDate.DayOfWeek;
                    var daysToMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
                    currentDate = currentDate.AddDays(-daysToMonday);

                    while (currentDate <= endDate)
                    {
                        var dueDate = GetEndOfWeek(currentDate);
                        tasks.Add(new Tasks(
                            request.Title,
                            request.Description,
                            dueDate,
                            request.Priority,
                            request.Type,
                            request.UserId
                        ));
                        currentDate = currentDate.AddDays(7);
                    }
                    break;

                case TaskType.Monthly:
                    // Ajustar para o primeiro dia do mês
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, 1);

                    while (currentDate <= endDate)
                    {
                        var dueDate = new DateTime(currentDate.Year, currentDate.Month, 
                            DateTime.DaysInMonth(currentDate.Year, currentDate.Month), 23, 59, 59, DateTimeKind.Utc);
                        tasks.Add(new Tasks(
                            request.Title,
                            request.Description,
                            dueDate,
                            request.Priority,
                            request.Type,
                            request.UserId
                        ));
                        currentDate = currentDate.AddMonths(1);
                    }
                    break;
            }

            return tasks;
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
            return DateTime.SpecifyKind(new DateTime(endOfWeek.Year, endOfWeek.Month, endOfWeek.Day, 23, 59, 59), DateTimeKind.Utc);
        }
    }
} 