using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using eHesabim.Core.Logging;
using eHesabim.Framework.Controllers;
using eHesabim.Web.Task.Engine;
using eHesabim.Web.Task.Models;

namespace eHesabim.Web.Task.Controllers {
    public class HomeController : Controller {
        private readonly ILogging logging;

        public HomeController(ILogging logging) {
            this.logging = logging;
        }

        public ActionResult Index() {
            logging.Trace("heartbeat");
            return View(GetModel(false));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("log")]
        public ActionResult GetLog() {
            return View(GetModel(true));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("stop")]
        public ActionResult StopTask() {
            TaskManager.Instance.Stop();
            logging.Trace("stopped");
            return View(GetModel(false));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("start")]
        public ActionResult StartTask() {
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
            logging.Trace("started");
            return View(GetModel(false));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("executetask")]
        public ActionResult ExecuteTask(int executetask) {
            TaskManager.Instance.Start(executetask);
            logging.Trace(string.Format("{0} task started", executetask));
            return View(GetModel(false));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("killtask")]
        public ActionResult KilTask(int killtask) {
            TaskManager.Instance.Kill(killtask);
            logging.Trace(string.Format("{0} task killed", killtask));
            return View(GetModel(false));
        }

        [HttpPost, ActionName("Index"), FormValueRequired("refresh")]
        public ActionResult Refresh() {
            return View(GetModel(false));
        }

        private HomeTaskModel GetModel(bool hasLog) {
            var lines = new List<string>();
            const int MaxLines = 100;

            if (hasLog) {
                var fileName = string.Format(@"c:\@logs\task_{0:yyyy-MM-dd}.log", DateTime.Today);
                using (var file = System.IO.File.OpenRead(fileName)) {
                    var streamReader = new StreamReader(file);
                    string line;
                    while ((line = streamReader.ReadLine()) != null) {
                        lines.Add(line);
                    }

                    lines.Reverse();
                }
            }

            var isRunning = TaskManager.Instance.IsRunning;
            var tasks = TaskManager.Instance.GetThreads();
            var taskModel = tasks.Select(a => {
                var values = a.Value.Split(';');
                return new TaskModel {
                    Id = a.Key,
                    TaskName = Convert.ToString(values[0]),
                    Description = Convert.ToString(values[1]),
                    Seconds = Convert.ToInt32(values[2]),
                    IsActive = Convert.ToBoolean(values[3]),
                    IsRunning = Convert.ToBoolean(values[4]),
                    LastRun = values[5],
                    LastStatus = values[6]
                };
            }).ToList();

            return new HomeTaskModel { Tasks = taskModel, Logs = lines.Take(MaxLines).ToList(), IsRunning = isRunning };
        }
    }
}
