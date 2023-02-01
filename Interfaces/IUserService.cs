
using TaskList.Models;

namespace TaskList.Interfaces


{
    public interface IUserService
    {

        List<User>? GetAll();
        User Get(int id);
        User GetCurrentUser(string token);
        void Add(User user);
        void Delete(int id);
        int Count { get; }

    }

}