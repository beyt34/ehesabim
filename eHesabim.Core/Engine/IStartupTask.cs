namespace eHesabim.Core.Engine {
    public interface IStartupTask {
        int Order { get; }

        void Execute();
    }
}
