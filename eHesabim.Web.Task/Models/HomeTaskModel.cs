using System.Collections.Generic;

namespace eHesabim.Web.Task.Models {
    public class HomeTaskModel {
        public List<TaskModel> Tasks { get; set; }

        public List<string> Logs { get; set; }

        public bool IsRunning { get; set; }
    }
}