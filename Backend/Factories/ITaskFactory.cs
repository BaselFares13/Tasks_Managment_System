using Backend.Models;

namespace Backend.Factories
{
    public interface ITaskFactory
    {
        TaskModel CreateFromAdd(DTOs.AddTaskDTO dto);
        TaskModel CreateFromUpdate(DTOs.UpdateTaskDTO dto);
    }
}
