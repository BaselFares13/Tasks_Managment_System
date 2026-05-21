using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs
{
    public class UpdateTaskDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Deadline is required.")]
        public DateTime? Deadline { get; set; }
        
        [Required(ErrorMessage = "Priority is required.")]
        public Priority? Priority { get; set; }
        
        [Required(ErrorMessage = "Status is required.")]
        public Models.TaskStatus? Status { get; set; }
    }
}
