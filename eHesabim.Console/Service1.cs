using System.ServiceProcess;

namespace eHesabim.Console {
    /// <summary>The service 1.</summary>
    public partial class Service1 : ServiceBase {
        public Service1() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Program.Start(args);
        }

        protected override void OnStop() {
            Program.Stop();
        }
    }
}
