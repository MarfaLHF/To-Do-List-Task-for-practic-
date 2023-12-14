using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Diagnostics;
using ToDoList.Data.Models;
using ToDoListWeb.Models;
using ToDoListWeb.Services;

namespace ToDoListWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InterfaceClient I_Client;

        public HomeController(ILogger<HomeController> logger, InterfaceClient i_Client)
        {
            _logger = logger;
            I_Client = i_Client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateTask()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm] ToDoList.Data.Models.Task task, int id)
        {
            await I_Client.CreateTask(task, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask([FromForm] int id)
        {
            await I_Client.DeleteTask(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await I_Client.GetTask(id);
            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromForm] ToDoList.Data.Models.Task task)
        {
            await I_Client.UpdateTask(task.Id, task);
            return RedirectToAction("TaskDetails", new { id = task.Id });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
