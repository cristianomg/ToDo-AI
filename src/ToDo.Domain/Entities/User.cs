using ToDo.Domain.Enums;

namespace ToDo.Domain.Entities
{
    public class User : BaseEntity
    {
        public User(int id, string name, Role role)
        {
            Id = id;
            Name = name;
            Role = role;
        }
        public string Name { get; private set; }
        public Role Role { get; private set; }
    }
}
