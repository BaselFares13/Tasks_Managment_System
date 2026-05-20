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
        public IActionResult GetAllTasks()
        {
            try
            {

                var createdTask = _dbContext.GetAll();

                return StatusCode(201,
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
    }
}
