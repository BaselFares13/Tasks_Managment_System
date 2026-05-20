using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DbContext.Interfaces;
using Backend.Models;

namespace Backend.DbContext
{
    public class JsonDbContext : IDbContext
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "tasks.json");
        private List<TaskModel> _tasks;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public JsonDbContext()
        {
            _tasks = Load();
        }
        public IList<TaskModel> GetAll() { return _tasks.OrderByDescending(t =>t.Deadline).ToList(); }
        public TaskModel Add(TaskModel task) {
            
            if(!Enum.IsDefined(typeof(Priority), task.Priority))
                throw new ValidationException("Invalid task priority value.");

            if (!Enum.IsDefined(typeof(Models.TaskStatus), task.Status))
                throw new ValidationException("Invalid task status value.");

            _tasks.Add(task);
            
            Save();
            
            return task;
        }
        public TaskModel? GetById(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id);
        public TaskModel? Update(Guid id, TaskModel updated) {
            var existing = GetById(id);
            if (existing is null) return null;

            if (!Enum.IsDefined(typeof(Priority), updated.Priority))
                throw new ValidationException("Invalid task priority value.");

            if (!Enum.IsDefined(typeof(Models.TaskStatus), updated.Status))
                throw new ValidationException("Invalid task status value.");

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Deadline = updated.Deadline;
            existing.Priority = updated.Priority;
            existing.Status = updated.Status;

            Save();
            return existing;
        }
        public bool Delete(Guid id) { return false; }
        public TaskModel? MarkCompleted(Guid id) {
            var task = GetById(id);
            if (task is null) return null;

            task.Status = Models.TaskStatus.Completed;
            Save();
            return task;
        }
        public void Save() {
            var json = JsonSerializer.Serialize(_tasks, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        public string ExportToText() { return ""; }
        private List<TaskModel> Load() {
            if (!File.Exists(_filePath)) 
                throw new Exception("File not found: " + _filePath);

            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json)) return new List<TaskModel>();

            var tasks = JsonSerializer.Deserialize<List<TaskModel>>(json, _jsonOptions) ?? new List<TaskModel>();
            return tasks;
        }

    }
}
