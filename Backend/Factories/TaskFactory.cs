using Backend.DTOs;
using Backend.Models;

namespace Backend.Factories
{
    public class TaskFactory : ITaskFactory
    {
        public TaskModel CreateFromAdd(AddTaskDTO dto)
        {
            return new TaskModel
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline ?? DateTime.UtcNow,
                Priority = dto.Priority ?? Priority.Medium
            };
        }

        public TaskModel CreateFromUpdate(UpdateTaskDTO dto)
        {
            return new TaskModel
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline ?? DateTime.UtcNow,
                Priority = dto.Priority ?? Priority.Medium,
                Status = dto.Status ?? Models.TaskStatus.ToDo
            };
        }
    }
}
