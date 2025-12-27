using System.ComponentModel.DataAnnotations;

namespace SimpleTaskManagementWebApplication.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        [Required(ErrorMessage = "Task Title is Required.")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DueDate { get; set; }
    }
}
