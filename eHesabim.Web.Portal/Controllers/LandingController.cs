using System;
using System.Net;
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
        
        /// <summary>The heartbeat.</summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Heartbeat() {
            var sum = 0;

            // admin
            var message1 = "task.ehesabim OK";
            try {
                var request1 = WebRequest.Create("http://task.ehesabim.com");
                var response1 = request1.GetResponse();
                if (((HttpWebResponse)response1).StatusCode == HttpStatusCode.OK) {
                    sum += 1;
                }
            }
            catch (Exception) {
                message1 = "task.ehesabim Error";
            }

            var message2 = "task.coinprice OK";
            try {
                var request1 = WebRequest.Create("http://task.coinprice.cash");
                var response1 = request1.GetResponse();
                if (((HttpWebResponse)response1).StatusCode == HttpStatusCode.OK) {
                    sum += 1;
                }
            }
            catch (Exception) {
                message2 = "task.coinprice Error";
            }

            // ok
            if (sum == 2) {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            // error
            var message = string.Format("{0}|{1}|{2}", sum, message1, message2);
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
        }
    }
}