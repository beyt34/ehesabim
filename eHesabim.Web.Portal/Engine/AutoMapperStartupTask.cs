using eHesabim.Core.Engine;

namespace eHesabim.Web.Portal.Engine {
    public class AutoMapperStartupTask : IStartupTask {
        public int Order {
            get { return 1; }
        }

        public void Execute() {
            AutoMapperConfiguration.Init();
        }
    }
}
