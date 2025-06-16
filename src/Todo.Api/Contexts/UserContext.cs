using ToDo.Domain.Enums;

namespace Todo.Api.Contexts
{
    public class UserContext
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Role Role { get; set; }
    }

    public interface IUserContextAccessor
    {
        UserContext? CurrentUser { get; set; }
    }

    public class UserContextAccessor : IUserContextAccessor
    {
        public UserContext? CurrentUser { get; set; }
    }
} 