using System;
using System.Threading;

namespace eHesabim.Core.Tasks {
    public class TaskThread {
        private readonly int seconds;

        private readonly Task task;

        private bool isRunning;

        private Timer timer;

        private bool disposed;

        private DateTime started;

        public TaskThread(ScheduledTaskModel scheduleTask) {
            task = new Task(scheduleTask);
            seconds = scheduleTask.ScheduledTaskSeconds;
            if (scheduleTask.ScheduledTaskLastSuccess.HasValue) {
                started = Convert.ToDateTime(scheduleTask.ScheduledTaskLastSuccess);
            }
        }

        public delegate void SetLastRun(Task task);

        public event SetLastRun SetLastRunEvent;

        public int Interval {
            get {
                return seconds * 1000;
            }
        }

        public int Seconds {
            get {
                return seconds;
            }
        }

        public DateTime Started {
            get {
                return started;
            }
        }

        public bool IsRunning {
            get {
                return isRunning;
            }
        }

        public int Id {
            get {
                return task.Id;
            }
        }

        public string Name {
            get {
                return task.Name;
            }
        }

        public string Description {
            get {
                return task.Description;
            }
        }

        public bool IsActive {
            get {
                return timer != null;
            }
        }

        public string LastStatus {
            get {
                return !string.IsNullOrEmpty(task.LastStatus) ? task.LastStatus : "&nbsp;";
            }
        }

        public void InitTimer() {
            if (timer == null) {
                timer = new Timer(TimerHandler, null, Interval, Interval);
            }
        }

        public void Dispose() {
            if ((timer != null) && !disposed) {
                lock (this) {
                    timer.Dispose();
                    timer = null;
                    disposed = true;
                }
            }
        }

        public void Run() {
            if (seconds <= 0) {
                return;
            }

            started = DateTime.Now;
            isRunning = true;
            task.Execute();
            OnSetLastRunEvent(task);
            isRunning = false;
        }

        public void Kill() {
        }

        protected virtual void OnSetLastRunEvent(Task task1) {
            var handler = SetLastRunEvent;
            if (handler != null) {
                handler(task1);
            }
        }

        private void TimerHandler(object state) {
            timer.Change(-1, -1);
            Run();
            if (timer != null) {
                timer.Change(Interval, Interval);
            }
        }
    }
}
