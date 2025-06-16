using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Domain.Repositories;

namespace ToDo.Infrastructure.Repositories
{
    public class TaskRepository : BaseRepository<Tasks>, ITaskRepository
    {
        public TaskRepository(DataContext context) : base(context)
        {
        }

        public async Task<Tasks?> GetByIdWithDetailsAsync(int id)
        {
            return await GetByIdAsync(id, t => t.User);
        }
        public async Task<IEnumerable<Tasks>> GetByTypeAndDateAndUserAsync(TaskType type, DateTime date, int userId)
        {
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var query = GetQueryable().Where(t => t.UserId == userId);

            var tasks = type switch
            {
                TaskType.Daily => await query
                    .Where(t => t.Type == TaskType.Daily &&
                               t.CreatedAt.Date == startOfDay.Date)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.CreatedAt)
                    .ToListAsync(),

                TaskType.Weekly => await query
                    .Where(t => t.Type == TaskType.Weekly &&
                               t.CreatedAt.Date >= GetStartOfWeek(startOfDay) &&
                               t.CreatedAt.Date <= GetEndOfWeek(startOfDay))
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.CreatedAt)
                    .ToListAsync(),

                TaskType.Monthly => await query
                    .Where(t => t.Type == TaskType.Monthly &&
                               t.CreatedAt.Year == startOfDay.Year &&
                               t.CreatedAt.Month == startOfDay.Month)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.CreatedAt)
                    .ToListAsync(),

                _ => throw new ArgumentException("Invalid task type", nameof(type))
            };

            return tasks;
        }
        private DateTime GetStartOfWeek(DateTime date)
        {
            // Get the current day of week (0 = Sunday, 6 = Saturday)
            var currentDayOfWeek = (int)date.DayOfWeek;
            
            // Calculate days until Monday (start of week)
            var daysUntilStartOfWeek = currentDayOfWeek == 0 ? 6 : currentDayOfWeek - 1;
            
            // Subtract days to get to Monday
            return date.AddDays(-daysUntilStartOfWeek).Date;
        }

        private DateTime GetEndOfWeek(DateTime date)
        {
            // Get the current day of week (0 = Sunday, 6 = Saturday)
            var currentDayOfWeek = (int)date.DayOfWeek;
            
            // Calculate days until Sunday (end of week)
            var daysUntilEndOfWeek = 7 - currentDayOfWeek;
            
            // Add days to get to Sunday
            return date.AddDays(daysUntilEndOfWeek).Date;
        }
    }
} 