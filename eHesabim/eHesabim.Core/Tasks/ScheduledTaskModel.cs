using System;

namespace eHesabim.Core.Tasks {
    public class ScheduledTaskModel {
        public int ScheduledTaskId { get; set; }

        public string ScheduledTaskName { get; set; }

        public string ScheduledTaskDll { get; set; }

        public string ScheduledTaskDescription { get; set; }

        public int ScheduledTaskSeconds { get; set; }

        public DateTime? ScheduledTaskLastSuccess { get; set; }

        public bool ScheduledTaskStopOnError { get; set; }

        public bool ScheduledTaskIsActive { get; set; }
    }
}
