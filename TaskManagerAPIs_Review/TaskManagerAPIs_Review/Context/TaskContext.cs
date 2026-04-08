using Microsoft.EntityFrameworkCore;
using TaskManagerAPIs_Review.Models;
using Task = TaskManagerAPIs_Review.Models.Task;

namespace TaskManagerAPIs_Review.Context
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
