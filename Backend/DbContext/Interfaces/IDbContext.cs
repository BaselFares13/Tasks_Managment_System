using Backend.Models;

namespace Backend.DbContext.Interfaces
{
    public interface IDbContext
    {
        public IList<TaskModel> GetAll();
        public TaskModel Add(TaskModel task);
        public TaskModel? GetById(Guid id);
        public TaskModel? Update(Guid id, TaskModel updated);
        public bool Delete(Guid id);
        public TaskModel? MarkCompleted(Guid id);
        public void Save();
        public string ExportToText();
    }
}
