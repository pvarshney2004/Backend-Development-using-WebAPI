using Microsoft.Extensions.Caching.Distributed;
using System.Formats.Asn1;
using System.Text.Json;
using TaskManagerAPIs_Review.DTOs.Task;
using TaskManagerAPIs_Review.Repositories.Interface;
using TaskManagerAPIs_Review.Services.Interface;

namespace TaskManagerAPIs_Review.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        private readonly IDistributedCache _cache;
        public TaskService(ITaskRepository taskRepository, IDistributedCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }
        public async Task<Models.Task> CreateTask(CreateTaskDTO createTaskDTO, int userId)
        {
            Models.Task task = new Models.Task
            {
                Title = createTaskDTO.Title,
                Description = createTaskDTO.Description,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            _taskRepository.CreateTask(task);

            string cacheKey = $"tasks_{userId}";
            await _cache.RemoveAsync(cacheKey);
            return task;
        }

        public async Task<bool> DeleteTask(int taskId, int userId)
        {
            var ans =  _taskRepository.DeleteTask(taskId, userId);
            string cacheKey = $"tasks_{userId}";
            await _cache.RemoveAsync(cacheKey);
            return ans;
        }

        public async Task<List<Models.Task>> GetAllTasks(int userId)
        {
            string cacheKey = $"tasks_{userId}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<Models.Task>>(cachedData);
            }


            var tasks = _taskRepository.GetAllTasks(userId);

            var cacheOptions = new DistributedCacheEntryOptions()
                               .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            var serializedTasks = JsonSerializer.Serialize(tasks);
            await _cache.SetStringAsync(cacheKey, serializedTasks, cacheOptions);

            return tasks;
        }

        public Models.Task GetTaskById(int taskId, int userId)
        {
            return _taskRepository.GetTaskById(taskId, userId);
        }

        public bool MarkComplete(int taskId, int userId)
        {
            return _taskRepository.MarkComplete(taskId, userId);
        }
        public async Task<Models.Task> UpdateTask(int taskId, UpdateTaskDTO updateTaskDTO, int userId)
        {
            var task = _taskRepository.GetTaskById(taskId, userId);
            if(task == null)
            {
                return null;
            }
            task.Title = updateTaskDTO.Title;
            task.Description = updateTaskDTO.Description;
            task.IsCompleted = updateTaskDTO.IsCompleted;

            _taskRepository.UpdateTask(task, userId);

            string cacheKey = $"tasks_{userId}";
            await _cache.RemoveAsync(cacheKey);
            return task;
        }
    }
}
