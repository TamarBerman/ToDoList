using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskList.Models;
using TaskList.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace TaskList.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyTaskController : ControllerBase
    {

        ITaskService TaskService;
        public MyTaskController(ITaskService TaskService)
        {
            this.TaskService = TaskService;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public ActionResult<List<MyTask>> GetAll()
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return TaskService.GetAll(token);
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public ActionResult<MyTask> Get(int id)
        {
            var task = TaskService.Get(id);

            if (task == null)
                return NotFound();

            return task;
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        public IActionResult Create(MyTask task)
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            TaskService.Add(task, token);
            return CreatedAtAction(nameof(Create), new { id = task.Id }, task);
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult Update(int id, MyTask task)
        {
            if (id != task.Id)
                return BadRequest();
            var existingTask = TaskService.Get(id);
            if (existingTask is null)
                return NotFound();
            TaskService.Update(task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult Delete(int id)
        {
            var task = TaskService.Get(id);
            if (task is null)
                return NotFound();
            TaskService.Delete(id);
            return Content(TaskService.Count.ToString());
        }
    }
}