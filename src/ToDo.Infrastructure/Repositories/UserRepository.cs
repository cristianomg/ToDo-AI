using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;

namespace ToDo.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
} 