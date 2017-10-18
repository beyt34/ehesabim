using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Web.Portal.Engine;

namespace eHesabim.Web.Portal.Controllers {
    public class LandingController : Controller {
        private readonly IWorkContext workContext;

        public LandingController(IWorkContext workContext) {
            this.workContext = workContext;
        }

        public ActionResult Index() {
            if (workContext != null && workContext.CurrentUser != null) {
                return RedirectToRoute(RouteNames.Home);
            }

            return View();
        }
    }
}