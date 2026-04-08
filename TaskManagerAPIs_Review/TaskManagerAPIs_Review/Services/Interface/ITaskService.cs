using TaskManagerAPIs_Review.DTOs.Task;
using TaskManagerAPIs_Review.Models;

namespace TaskManagerAPIs_Review.Services.Interface
{
    public interface ITaskService
    {
        Task<Models.Task> CreateTask(CreateTaskDTO createTaskDTO, int userId);

        Task<List<Models.Task>> GetAllTasks(int userId);

        Models.Task GetTaskById(int taskId, int userId);

        Task<Models.Task> UpdateTask(int taskId, UpdateTaskDTO updateTaskDTO, int userId);

        bool MarkComplete(int taskId, int userId);

        Task<bool> DeleteTask(int taskId, int userId);
    }
}
