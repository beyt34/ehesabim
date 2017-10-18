using System.IO;
using System.Web.Mvc;

namespace eHesabim.Framework {
    public static class ContextExtensions {
        public static string RenderRazorViewToString(this ControllerContext controllerContext, string viewName, object model) {
            if (controllerContext == null) {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(viewName)) {
                viewName = controllerContext.RouteData.GetRequiredString("action");
            }

            controllerContext.Controller.ViewData.Model = model;
            using (var sw = new StringWriter()) {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
