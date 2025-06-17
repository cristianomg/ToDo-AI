using ToDo.Domain.Enums;

namespace ToDo.Domain.DTOs
{
    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TasksStatus Status { get; set; }
        public TaskType Type { get; set; }
        public int UserId { get; set; }
        public List<ChecklistItemDto> Checklist { get; set; } = new();
    }
}
