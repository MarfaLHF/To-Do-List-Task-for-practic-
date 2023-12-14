using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Data.Models;
using ToDoListWeb.Models;
using ToDoListWeb.Services;

namespace ToDoListWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly InterfaceClient _userApiClient;

        public int userId;
        public AccountController(ILogger<AccountController> logger, InterfaceClient userApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDTO loginModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    string token = await _userApiClient.Login(loginModel);
                    userId = await _userApiClient.GetUserIdByUsername(loginModel.Username);

                    return RedirectToAction("Index", new { id = userId });
                  
                   

                    
                    
                }
                catch (Exception ex)
                {
                   
                    _logger.LogError(ex, "Login failed");
                    ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                }
            }

            
            return View(loginModel);
        }

        public async Task<IActionResult> Index(int id)
        {
            
            var tasks = await _userApiClient.GetAllTasks(id);
            userId = id;
            ViewData["userId"] = userId;

            return View(tasks);
            
        }







        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await RegisterUser(user);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    HandleRegistrationError(ex);
                }
            }

            return View(user);
        }

 
      

        private async System.Threading.Tasks.Task RegisterUser(User user)
        {
            await _userApiClient.RegisterUser(new User
            {
                Username = user.Username,
                PasswordHash = user.PasswordHash,
            });
        }

        private void HandleRegistrationError(Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
        }




        public async Task<IActionResult> CreateTask()
        {
            
            //await _userApiClient.GetAllTasks(Id);

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm] ToDoList.Data.Models.Task task, [FromRoute] int id)
        {
            await _userApiClient.CreateTask(task, id);
            return RedirectToAction("Index", "Account", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask([FromForm] int id)
        {
            await _userApiClient.DeleteTask(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _userApiClient.GetTask(id);

            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromForm] ToDoList.Data.Models.Task task)
        {
            await _userApiClient.UpdateTask(task.Id, task);
            return RedirectToAction("Index", new { id = task.UserId });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
