using System;

using eHesabim.Core.Engine;
using eHesabim.Core.Logging;

namespace eHesabim.Core.Tasks {
    public class Task {
        private readonly ILogging logging;

        private readonly Type taskType;

        private readonly int id;

        private readonly string name;
        
        private readonly string description;

        private readonly bool stopOnError;

        private ITask task;

        private bool enabled;

        private DateTime? lastStarted;

        private DateTime? lastSuccess;

        private DateTime? lastEnd;

        private bool isRunning;

        public Task() {
            logging = EngineContext.Current.Resolve<ILogging>();
        }

        public Task(ScheduledTaskModel task) : this() {
            taskType = Type.GetType(task.ScheduledTaskDll);
            enabled = task.ScheduledTaskIsActive;
            stopOnError = task.ScheduledTaskStopOnError;
            id = task.ScheduledTaskId;
            name = task.ScheduledTaskName;
            description = task.ScheduledTaskDescription;

            if (task.ScheduledTaskLastSuccess.HasValue) {
                lastStarted = Convert.ToDateTime(task.ScheduledTaskLastSuccess);
            }

            lastSuccess = task.ScheduledTaskLastSuccess;
        }

        public bool IsRunning {
            get {
                return isRunning;
            }
        }

        public DateTime? LastStarted {
            get {
                return lastStarted;
            }
        }

        public DateTime? LastEnd {
            get {
                return lastEnd;
            }
        }

        public DateTime? LastSuccess {
            get {
                return lastSuccess;
            }
        }

        public Type TaskType {
            get {
                return taskType;
            }
        }

        public bool StopOnError {
            get {
                return stopOnError;
            }
        }

        public int Id {
            get {
                return id;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Description {
            get {
                return description;
            }
        }

        public bool Enabled {
            get {
                return enabled;
            }
        }

        public string LastStatus { get; set; }

        public void Execute() {
            isRunning = true;
            try {
                var task1 = CreateTask();
                if (task1 != null) {
                    lastStarted = DateTime.Now;
                    task1.Execute(lastSuccess);
                    lastSuccess = lastStarted;
                    lastEnd = DateTime.Now;
                    LastStatus = "Success";
                }
            }
            catch (Exception exception) {
                logging.Error(exception);
                enabled = !StopOnError;
                lastEnd = DateTime.Now;
                LastStatus = string.Format("{0} - {1}", lastEnd, exception.Message);
            }

            isRunning = false;
        }

        private ITask CreateTask() {
            if (Enabled && (task == null)) {
                if (taskType == null) {
                    enabled = false;
                    logging.Error("{0} Task Not Created, dll not found", taskType);
                    return null;
                }

                task = Activator.CreateInstance(taskType) as ITask;
                enabled = task != null;
            }

            return task;
        }
    }
}