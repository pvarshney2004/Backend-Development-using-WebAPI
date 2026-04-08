
using Task = TaskManagerAPIs_Review.Models.Task;

namespace TaskManagerAPIs_Review.Repositories.Interface
{
    public interface ITaskRepository
    {
        Task CreateTask(Task task);
        List<Task> GetAllTasks(int userId);

        Task GetTaskById(int taskId, int userId);

        Task UpdateTask(Task task, int userId);

        bool MarkComplete(int taskId, int userId);

        bool DeleteTask(int taskId, int userId);
    }
}
