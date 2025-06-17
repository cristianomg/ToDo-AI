using ToDo.Domain.Enums;

namespace ToDo.Domain.Entities
{
    public class Tasks : BaseEntity
    {
        public Tasks(string title, string? description, DateTime dueDate, TaskPriority priority, TaskType type, int userId)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = TasksStatus.Pending;
            Type = type;
            UserId = userId;
        }

        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TasksStatus Status { get; private set; }
        public TaskType Type { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }

        public ICollection<ChecklistItem> Checklist { get; set; } = new List<ChecklistItem>();

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            Title = title;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(TasksStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
