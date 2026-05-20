using System.Text.Json;
using Backend.DbContext.Interfaces;
using Backend.Models;

namespace Backend.DbContext
{
    public class JsonDbContext : IDbContext
    {
        private readonly string _filePath = "tasks.json";
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
        public IList<TaskModel> GetAll() { return new List<TaskModel>(); }
        public TaskModel Add(TaskModel task) { return task; }
        public TaskModel? GetById(Guid id) { return null; }
        public TaskModel? Update(Guid id, TaskModel updated) { return null; }
        public bool Delete(Guid id) { return false; }
        public TaskModel? MarkCompleted(Guid id) { return new TaskModel(); }
        public void Save() { }
        public string ExportToText() { return ""; }
        private List<TaskModel> Load() { return new List<TaskModel>(); }

    }
}
