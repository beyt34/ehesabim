using System;

using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Tasks;

namespace eHesabim.Tasks {
    public class TestTask : ITask {
        private readonly ILogging logging = EngineContext.Current.Resolve<ILogging>();

        public void Execute(DateTime? lastSuccess) {
            logging.Info("TestTask Started");
        }
    }
}
