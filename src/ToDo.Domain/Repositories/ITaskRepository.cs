using ToDo.Domain.Entities;
using ToDo.Domain.Enums;

namespace ToDo.Domain.Repositories
{
    public interface ITaskRepository : IBaseRepository<Tasks>
    {
        Task<Tasks?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Tasks>> GetByTypeAndDateAndUserAsync(TaskType type, DateTime date, int userId);
    }
}
