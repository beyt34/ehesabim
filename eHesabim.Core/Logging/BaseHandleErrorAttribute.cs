using System.Web.Mvc;
using eHesabim.Core.Engine;

namespace eHesabim.Core.Logging {
    public class BaseHandleErrorAttribute : HandleErrorAttribute {
        public override void OnException(ExceptionContext context) {
            if (!context.ExceptionHandled) {
                // if unhandled, will be logged anyhow
                return;
            }

            base.OnException(context);
            var logger = EngineContext.Current.Resolve<ILogging>();
            logger.Error(context.Exception);
        }
    }
}
