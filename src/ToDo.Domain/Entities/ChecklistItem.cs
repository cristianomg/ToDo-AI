using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Domain.Entities
{
    public class ChecklistItem : BaseEntity
    {
        public ChecklistItem() { }
        public ChecklistItem(string text, int taskId)
        {
            Text = text;
            Completed = false;
            TaskId = taskId;
        }
        public string Text { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public int TaskId { get; set; }
        public Tasks Task { get; set; }
    }
} 