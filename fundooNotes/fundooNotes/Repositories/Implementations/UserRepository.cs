using fundooNotes.Context;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;

namespace fundooNotes.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        // _context gives access to database tables.
        private readonly FundooContext _context;
        public UserRepository(FundooContext context)
        {
            _context = context;
        }

        public User Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetUserByResetToken(string token)
        {
            return _context.Users
                .FirstOrDefault(u => u.ResetToken == token);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void CreateSession(UserSession session)
        {
            _context.UserSessions.Add(session);
            _context.SaveChanges();
        }

        public void Logout(string token)
        {
            var session = _context.UserSessions
                .FirstOrDefault(s => s.Token == token && s.IsActive);
            if (session != null)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}
