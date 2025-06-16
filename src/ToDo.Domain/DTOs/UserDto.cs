using ToDo.Domain.Enums;

namespace ToDo.Domain.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}