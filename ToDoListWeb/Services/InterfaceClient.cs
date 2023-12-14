using Refit;
using ToDoList.Data.Models;

namespace ToDoListWeb.Services
{
    public interface InterfaceClient
    {
        #region Task
        [Get("/Task")]
        Task<IEnumerable<ToDoList.Data.Models.Task>> GetAllTasks([Query] int userId);

        [Get("/Task/{id}")]
        Task<ToDoList.Data.Models.Task> GetTask(int id);

        [Post("/Task")]
        System.Threading.Tasks.Task CreateTask([Body] ToDoList.Data.Models.Task task, int id);

        [Put("/Task/{id}")]
        System.Threading.Tasks.Task UpdateTask(int id, [Body] ToDoList.Data.Models.Task task);

        [Delete("/Task/{id}")]
        System.Threading.Tasks.Task DeleteTask(int id);
        #endregion

        #region User
        [Post("/User/register")]
        System.Threading.Tasks.Task RegisterUser([Body] User user);

        [Get("/User/userinfo/{id}")]
        Task<User> GetUserInfo(int id);

        [Post("/User/login")]
        Task<string> Login([Body] UserDTO request);

        [Get("/User/GetUserId")]
        Task<int> GetUserIdByUsername([Query] string username);
        #endregion
    }
}
