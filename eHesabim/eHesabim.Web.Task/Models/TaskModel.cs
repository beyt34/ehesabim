namespace eHesabim.Web.Task.Models {
    public class TaskModel {
        public int Id { get; set; }

        public string TaskName { get; set; }

        public string Description { get; set; }

        public int Seconds { get; set; }

        public bool IsActive { get; set; }

        public bool IsRunning { get; set; }

        public string LastRun { get; set; }

        public string LastStatus { get; set; }
    }
}