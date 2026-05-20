using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        public IList<TaskModel> GetAll() { return _tasks; }
        public TaskModel Add(TaskModel task) {
            
            if(!Enum.IsDefined(typeof(Priority), task.Priority))
                throw new ValidationException("Invalid task priority value.");

            if (!Enum.IsDefined(typeof(Models.TaskStatus), task.Status))
                throw new ValidationException("Invalid task status value.");

            _tasks.Add(task);
            
            Save();
            
            return task;
        }
        public TaskModel? GetById(Guid id) { return null; }
        public TaskModel? Update(Guid id, TaskModel updated) { return null; }
        public bool Delete(Guid id) { return false; }
        public TaskModel? MarkCompleted(Guid id) { return new TaskModel(); }
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
