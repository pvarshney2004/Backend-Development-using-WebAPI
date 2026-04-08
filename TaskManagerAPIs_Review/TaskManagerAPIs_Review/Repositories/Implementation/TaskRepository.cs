using TaskManagerAPIs_Review.Context;
using TaskManagerAPIs_Review.DTOs.Task;
using TaskManagerAPIs_Review.Models;
using TaskManagerAPIs_Review.Repositories.Interface;

namespace TaskManagerAPIs_Review.Repositories.Implementation
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskContext _context;
        public TaskRepository(TaskContext context)
        {
            _context = context;
        }

        public Models.Task CreateTask(Models.Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return task;
        }

        public List<Models.Task> GetAllTasks(int userId)
        {
            return _context.Tasks.Where(t => t.UserId == userId).ToList();
        }

        public Models.Task GetTaskById(int taskId, int userId)
        {
            return _context.Tasks.FirstOrDefault(t => (t.Id == taskId && t.UserId == userId));
        }

        public bool MarkComplete(int taskId, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);
            if (task == null) return false;
            task.IsCompleted = true;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteTask(int taskId, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        public Models.Task UpdateTask(Models.Task task, int userId)
        {
            var taskExist = _context.Tasks.FirstOrDefault(t => t.Id == task.Id && t.UserId == userId);
            if (taskExist == null) return null;
            taskExist.Title = task.Title;
            taskExist.Description = task.Description;
            taskExist.IsCompleted = task.IsCompleted;
            _context.SaveChanges();
            return taskExist;
        }
    }
}
