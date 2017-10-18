using System;

using NLog;

namespace eHesabim.Core.Logging {
    public class Logging : ILogging {
        private readonly Logger logger;

        public Logging() {
            logger = LogManager.GetCurrentClassLogger();
            ////logger = LogManager.GetLogger("test");
        }

        public static ILogging GetLoggingService() {
            var logger = (ILogging)LogManager.GetLogger("NLogLogger", typeof(Logger));
            return logger;
        }

        public void Trace(string message) {
            logger.Log(LogLevel.Trace, message);
        }

        public void Trace(string format, params object[] args) {
            logger.Trace(string.Format(format, args));
        }

        public void Info(string message) {
            logger.Log(LogLevel.Info, message);
        }

        public void Info(string format, params object[] args) {
            logger.Info(string.Format(format, args));
        }

        public void Info(Exception exception) {
            logger.Info(exception);
        }

        public void Debug(Exception exception) {
            logger.Debug(exception);
        }

        public void Debug(string format, params object[] args) {
            logger.Debug(string.Format(format, args));
        }

        public void Debug(Exception exception, string format, params object[] args) {
            logger.Debug(format, args);
            logger.Debug(exception);
        }

        public void Error(string format, params object[] args) {
            logger.Error(string.Format(format, args));
        }

        public void Error(Exception exception) {
            logger.Error(exception);
        }

        public void Fatal(Exception exception) {
            logger.Fatal(exception);
        }
    }
}
