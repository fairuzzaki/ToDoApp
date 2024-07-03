using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoesController : ControllerBase
    {
        private readonly DataContext _context;
        public ToDoesController(DataContext context) { _context = context; }

        // GET list todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetTodoList()
        {
            return await _context.Todo.ToListAsync();
        }

        // GET todo by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetTodo(int id)
        {
            var toDoItem = await _context.Todo.FindAsync(id);
            if (toDoItem == null) {
                return NotFound();
            }

            return toDoItem;
        }

        // Create new todo
        [HttpPost]
        public async Task<ActionResult<ToDo>> createTodo(ToDo toDo)
        {
            _context.Todo.Add(toDo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = toDo.Id }, toDo);
        }

        // Update todo by id
        [Route("update/{id}")]
        [HttpPut]
        public async Task<ActionResult<ToDo>> updateTodo(int id, ToDo toDo)
        {
            toDo.Id = id;
            if (id != toDo.Id) {
                return BadRequest();
            }

            _context.Entry(toDo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetTodo), new { id = toDo.Id }, toDo);
        }

        // Check if todo is exist
        private bool TodoExists(int id)
        {
            return _context.Todo.Any(e => e.Id == id);
        }

        // DELETE todo by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todoItem = await _context.Todo.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Todo.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
