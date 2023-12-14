namespace ToDoList.Data.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }

    }

}
