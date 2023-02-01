namespace TaskList.Models
{
    public class MyTask
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsDone { get; set; }
         public int UserId { get; set; }
    }
}
