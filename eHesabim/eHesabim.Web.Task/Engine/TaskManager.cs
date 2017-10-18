using System.Collections.Generic;
using System.Linq;

using eHesabim.Core.Tasks;

namespace eHesabim.Web.Task.Engine {
    public class TaskManager {
        private static readonly TaskManager TaskManagerr = new TaskManager();

        private static bool started;

        ////private readonly ICommonService commonService = EngineContext.Current.Resolve<ICommonService>();

        private List<TaskThread> taskThreads;

        private TaskManager() {
        }

        public static TaskManager Instance {
            get {
                return TaskManagerr;
            }
        }

        public bool IsRunning {
            get {
                return started;
            }
        }

        public void Initialize() {
            if (IsRunning) {
                return;
            }

            var scheduleTasks = new List<ScheduledTaskModel> {
                ////new ScheduledTaskModel {
                ////    ScheduledTaskId = 1,
                ////    ScheduledTaskName = "TestTask",
                ////    ScheduledTaskDll = "eHesabim.Tasks.TestTask, eHesabim.Tasks",
                ////    ScheduledTaskSeconds = 5,
                ////    ScheduledTaskStopOnError = false,
                ////    ScheduledTaskIsActive = true,
                ////},
                new ScheduledTaskModel {
                    ScheduledTaskId = 1,
                    ScheduledTaskName = "EmailQueueSendTask",
                    ScheduledTaskDll = "eHesabim.Tasks.EmailQueueSendTask, eHesabim.Tasks",
                    ScheduledTaskSeconds = 60,
                    ScheduledTaskStopOnError = false,
                    ScheduledTaskIsActive = true,
                },
            };

            ////var scheduleTasks = commonService.GetAllScheduledTasks().Data;
            taskThreads = new List<TaskThread>();
            foreach (var scheduleTask in scheduleTasks) {
                // one thread, one task
                var taskThread = new TaskThread(scheduleTask);
                taskThread.SetLastRunEvent += SetLastSuccess;
                taskThreads.Add(taskThread);
            }
        }

        public void Start() {
            started = true;
            foreach (var taskThread in taskThreads) {
                taskThread.InitTimer();
            }
        }

        public void Stop() {
            foreach (var taskThread in taskThreads) {
                taskThread.Dispose();
            }

            started = false;
        }

        public Dictionary<int, string> GetThreads() {
            return taskThreads.ToDictionary(
                taskThread => taskThread.Id,
                taskThread =>
                string.Format(
                    "{0};{1};{2};{3};{4};{5};{6}",
                    taskThread.Name,
                    taskThread.Description,
                    taskThread.Interval / 1000,
                    taskThread.IsActive,
                    taskThread.IsRunning,
                    taskThread.Started.ToString("dd.MM.yyyy HH:mm:ss"),
                    taskThread.LastStatus));
        }

        public void Start(int taskId) {
            foreach (var taskThread in taskThreads.Where(item => item.Id == taskId)) {
                taskThread.Run();
            }
        }

        public void Kill(int taskId) {
            foreach (var taskThread in taskThreads.Where(item => item.Id == taskId)) {
                taskThread.Kill();
            }
        }

        private void SetLastSuccess(Core.Tasks.Task task) {
            if (task.LastStatus == "Success" && task.LastStarted.HasValue) {
                ////commonService.SetScheduledTaskLastSuccess(task.Id, task.LastStarted.Value);
            }
        }
    }
}