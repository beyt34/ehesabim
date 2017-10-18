using eHesabim.Core.Engine;

namespace eHesabim.Services {
    public class AutoMapperStartupTask : IStartupTask {
        public int Order {
            get { return 0; }
        }

        public void Execute() {
            AutoMapperConfiguration.Init();
        }
    }
}
