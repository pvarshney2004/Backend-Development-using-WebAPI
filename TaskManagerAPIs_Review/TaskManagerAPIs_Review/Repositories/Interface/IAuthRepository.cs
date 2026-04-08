using TaskManagerAPIs_Review.Models;

namespace TaskManagerAPIs_Review.Repositories.Interface
{
    public interface IAuthRepository
    {
        User Register(User user);

        User GetUserByUsername(string username);

    }
}
