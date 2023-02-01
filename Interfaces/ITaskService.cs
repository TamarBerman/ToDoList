
using TaskList.Models;

namespace TaskList.Interfaces


{
    public interface ITaskService
    {

        List<MyTask>? GetAll(String token);
        MyTask Get(int id);
        void Add(MyTask task,string token);
        void Delete(int id);
        void Update(MyTask task);
        int Count { get; }

    }

}