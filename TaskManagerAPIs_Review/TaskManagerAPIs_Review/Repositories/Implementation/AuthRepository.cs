using TaskManagerAPIs_Review.Context;
using TaskManagerAPIs_Review.Models;
using TaskManagerAPIs_Review.Repositories.Interface;

namespace TaskManagerAPIs_Review.Repositories.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TaskContext _context;
        public AuthRepository(TaskContext context)
        {
            _context = context;
        }

        public User Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }





    }
}
