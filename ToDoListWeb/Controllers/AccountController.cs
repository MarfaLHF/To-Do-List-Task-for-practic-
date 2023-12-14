using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
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

        // GET: Account/Index/{id}
        public async Task<IActionResult> Index(int id)
        {
            var tasks = await _userApiClient.GetAllTasks(id);
            userId = id;
            ViewData["userId"] = userId;

            return View(tasks);
        }

        // GET: Account/Registration
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }



        // POST: Account/Registration
        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Проверка на пустые поля
                    if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
                    {
                        ModelState.AddModelError(string.Empty, "Имя пользователя и пароль обязательны.");
                        return View(user);
                    }

                    // Регистрация нового пользователя
                    await RegisterUser(user);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Обработка ошибок регистрации
                    HandleRegistrationError(ex);
                }
            }

            return View(user);
        }


        // Private method to handle user registration
        private async System.Threading.Tasks.Task RegisterUser(User user)
        {
            await _userApiClient.RegisterUser(new User
            {
                Username = user.Username,
                PasswordHash = user.PasswordHash,
            });
        }

        // Private method to handle registration errors
        private void HandleRegistrationError(Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
        }

        // GET: Account/CreateTask
        public async Task<IActionResult> CreateTask()
        {
            return View();
        }

        // POST: Account/CreateTask/{id}
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm] ToDoList.Data.Models.Task task, [FromRoute] int id)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(task.Description))
            {
                ModelState.AddModelError(string.Empty, "Описание задачи обязательно.");
                return View(task);
            }

            // Создание новой задачи для пользователя
            await _userApiClient.CreateTask(task, id);
            return RedirectToAction("Index", "Account", new { id = id });
        }

        // POST: Account/DeleteTask
        [HttpPost]
        public async Task<IActionResult> DeleteTask([FromForm] ToDoList.Data.Models.Task task)
        {
            ToDoList.Data.Models.Task task1 = await _userApiClient.GetTask(task.Id);
            await _userApiClient.DeleteTask(task.Id);

            return RedirectToAction("Index", new { id = task1.UserId });
        }

        // GET: Account/TaskDetails/{id}
        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _userApiClient.GetTask(id);
            ViewData["userId"] = userId; // Ensure userId is set correctly

            return View(task);
        }

        // POST: Account/UpdateTask
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromForm] ToDoList.Data.Models.Task task)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(task.Description))
            {
                ModelState.AddModelError(string.Empty, "Описание задачи обязательно.");
                return View(task);
            }

            // Обновление информации о задаче
            await _userApiClient.UpdateTask(task.Id, task);
            return RedirectToAction("Index", new { id = task.UserId });
        }

        // GET: Account/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
