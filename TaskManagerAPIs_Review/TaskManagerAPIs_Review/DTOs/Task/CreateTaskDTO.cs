using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIs_Review.DTOs.Task
{
    public class CreateTaskDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "Description too long")]
        public string Description { get; set; }
    }
}
