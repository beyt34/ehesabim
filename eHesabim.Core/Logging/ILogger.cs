using System;

namespace eHesabim.Core.Logging {
    public interface ILogging {
        void Debug(Exception exception);

        void Debug(string format, params object[] args);

        void Debug(Exception exception, string format, params object[] args);

        void Trace(string message);

        void Info(string message);

        void Trace(string format, params object[] args);

        void Info(string format, params object[] args);

        void Error(string format, params object[] args);

        void Error(Exception exception);

        void Fatal(Exception exception);

        void Info(Exception exception);
    }
}
