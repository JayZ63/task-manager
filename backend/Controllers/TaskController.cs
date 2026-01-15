using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskContext _taskContext;
        public TaskController(TaskContext taskContext) => _taskContext = taskContext;

        [HttpGet]
        public async Task<IActionResult> GetTasks() => Ok(await _taskContext.Tasks.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> AddTask(TaskItem task)
        {
            _taskContext.Tasks.Add(task);
            await _taskContext.SaveChangesAsync();
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            var existing = await _taskContext.Tasks.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = task.Title;
            existing.IsCompleted = task.IsCompleted;
            await _taskContext.SaveChangesAsync();
            return Ok(existing);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            _taskContext.Tasks.Remove(task);
            await _taskContext.SaveChangesAsync();
            return Ok(task);
        }

    }
}
