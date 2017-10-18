using System;

namespace eHesabim.Core.Tasks {
    public interface ITask {
        void Execute(DateTime? lastSuccess);
    }
}
