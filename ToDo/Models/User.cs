namespace ToDo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; // default: User

        // Navigation property
        public List<TaskItem>? Tasks { get; set; } = new List<TaskItem>();

    }
}
