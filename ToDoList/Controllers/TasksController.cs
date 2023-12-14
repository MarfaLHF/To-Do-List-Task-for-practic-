using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly Context _context;

        public TaskController(Context context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        // В вашем TaskController
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoList.Data.Models.Task>>> GetTasks([FromQuery] int userId)
        {
            var tasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
            
            return Ok(tasks);
        }


        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<ToDoList.Data.Models.Task>> CreateTask(ToDoList.Data.Models.Task task, int id)
        {
            var newTask = new ToDoList.Data.Models.Task
            {
                UserId = id,
                Title = task.Title,
                Description = task.Description,
                Deadline = task.Deadline,
                Priority = task.Priority,
                IsCompleted = false
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }


        // GET: api/tasks/{taskId}
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoList.Data.Models.Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

 
            return Ok(task);
        }

        // PUT: api/tasks/{taskId}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, ToDoList.Data.Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest("Invalid task ID.");
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound("Task not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/tasks/{taskId}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(t => t.Id == id);
        }
    }
}


