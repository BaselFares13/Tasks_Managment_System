using System.ComponentModel.DataAnnotations;
using Backend.DbContext.Interfaces;
using Backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.DbContext;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tasksController : ControllerBase
    {
        private readonly IDbContext _dbContext;
        public tasksController()
        {
            _dbContext = JsonDbContextSingleton.GetInstance();
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] AddTaskDTO taskDto)
        {
            try
            {
                var task = new Models.TaskModel
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Deadline = taskDto.Deadline ?? DateTime.UtcNow,
                    Priority = taskDto.Priority ?? Models.Priority.Medium
                };

                var createdTask = _dbContext.Add(task);

                return StatusCode(201,
                    new
                    {
                        Success = true,
                        Data = createdTask
                    }
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(
                    new
                    {
                        Success = false,
                        Message = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpGet]
        public IActionResult GetAllTasks(bool orderByPriority = false)
        {
            try
            {

                var createdTask = _dbContext.GetAll();

                if(orderByPriority)
                    createdTask = createdTask.OrderByDescending(t => t.Priority).ToList();

                return Ok(
                    new
                    {
                        Success = true,
                        Data = createdTask
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpGet("Filter")]
        public IActionResult GetFilteredTasks(bool completed = false, bool highPrioritized = false)
        {
            try
            {

                var createdTask = _dbContext.GetAll();

                if (completed)
                    createdTask = createdTask.Where(t => t.Status == Models.TaskStatus.Completed).ToList();
                else
                    createdTask = createdTask.Where(t => t.Status == Models.TaskStatus.ToDo).ToList();

                if(highPrioritized)
                    createdTask = createdTask.Where(t => t.Priority == Models.Priority.High).ToList();

                return Ok(
                    new
                    {
                        Success = true,
                        Data = createdTask
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpPost("MarkCompleted/{id:required}")]
        public IActionResult MarkTaskAsCompleted(string id)
        {
            try
            {

                var MarkedTask = _dbContext.MarkCompleted(Guid.Parse(id));

                if (MarkedTask == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Task not found."
                    });

                return Ok(
                    new
                    {
                        Success = true,
                        Data = MarkedTask
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpPut("{id:required}")]
        public IActionResult UpdateTask(string id, [FromBody] UpdateTaskDTO updateTaskDTO)
        {
            try
            {
                var task = new Models.TaskModel
                {
                    Title = updateTaskDTO.Title,
                    Description = updateTaskDTO.Description,
                    Deadline = updateTaskDTO.Deadline ?? DateTime.UtcNow,
                    Priority = updateTaskDTO.Priority ?? Models.Priority.Medium,
                    Status = updateTaskDTO.Status ?? Models.TaskStatus.ToDo
                };

                var UpdatedTask = _dbContext.Update(Guid.Parse(id), task);

                if (UpdatedTask == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Task not found."
                    });

                return Ok(
                    new
                    {
                        Success = true,
                        Data = UpdatedTask
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpDelete("{id:required}")]
        public IActionResult DeleteTask(string id)
        {
            try
            {
                var result = _dbContext.Delete(Guid.Parse(id));

                if (result == false)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Task not found or task was not deleted."
                    });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

        [HttpGet("{id:required}")]
        public IActionResult GetTaskById(string id)
        {
            try
            {
                var Task = _dbContext.GetById(Guid.Parse(id));

                if (Task == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Task not found."
                    });

                return Ok(new
                {
                    Success = true,
                    Data = Task
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        Success = false,
                        Message = "An error occurred while creating the task.",
                        Details = ex.Message
                    }
                );
            }
        }

    }
}
