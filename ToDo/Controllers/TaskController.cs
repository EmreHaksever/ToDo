using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")
            {
                // Admin tüm taskları görebilir
                return await _context.Tasks.ToListAsync();
            }
            else
            {
                // Normal kullanıcı sadece kendi tasklarını görebilir
                return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            TaskItem task;
            if (userRole == "Admin")
            {
                task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            }
            else
            {
                task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            }

            if (task == null) return NotFound();
            return task;
        }

        // POST: api/task
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Task ekleyen kullanıcının id'sini ata
            task.UserId = userId;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/task/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskItem task)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (id != task.Id) return BadRequest();

            // Kullanıcı sadece kendi task'ını güncelleyebilir, Admin hepsini
            if (userRole != "Admin" && task.UserId != userId) return Forbid();

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(t => t.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }


        // DELETE: api/task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            // Kullanıcı sadece kendi task'ını silebilir, Admin hepsini
            if (userRole != "Admin" && task.UserId != userId) return Forbid();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}