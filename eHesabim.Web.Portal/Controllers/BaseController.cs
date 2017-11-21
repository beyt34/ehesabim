using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Framework.Localization;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Engine;
using Kendo.Mvc.UI;

namespace eHesabim.Web.Portal.Controllers {
    [Authorize]
    [NoCache]
    public class BaseController : Controller {
        private readonly ILogging logging = EngineContext.Current.Resolve<ILogging>();

        protected string SortField { get; set; }

        protected bool SortDescending { get; set; }

        [ActionName("Error_404")]
        public ActionResult Error404() {
            return View("~/Views/Common/Error_404.cshtml");
        }

        [ActionName("Error_500")]
        public ActionResult Error500() {
            return View("~/Views/Common/Error_500.cshtml");
        }

        [ActionName("AccessDenied")]
        public ActionResult AccessDenied() {
            return View("~/Views/Common/AccessDenied.cshtml");
        }

        protected string GetCurrencyString(decimal? value) {
            return string.Format("{0:N2} TL", value ?? 0);
        }

        protected void SetSort([DataSourceRequest]DataSourceRequest request) {
            if (request.Sorts != null && request.Sorts.Any()) {
                SortField = request.Sorts[0].Member;
                SortDescending = request.Sorts[0].SortDirection != ListSortDirection.Ascending;
            }
        }

        protected SelectList GetSelectList(IEnumerable<SelectGuidDataModel> list) {
            var tt = list.Select(a => new { Id = a.Id.ToString(), a.Name }).ToList();
            tt.Insert(0, new { Id = string.Empty, Name = Labels.Select });

            return new SelectList(tt, "Id", "Name");
        }

        protected SelectList GetSelectList(List<SelectIntDataModel> list) {
            list.Insert(0, new SelectIntDataModel { Id = 0, Name = Labels.Select });
            return new SelectList(list, "Id", "Name");
        }

        protected SelectList GetYesNoList() {
            var list = new List<SelectIntDataModel> {
                new SelectIntDataModel { Id = 0, Name = Labels.Select },
                new SelectIntDataModel { Id = 1, Name = Labels.Yes },
                new SelectIntDataModel { Id = 2, Name = Labels.No }
            };
            return new SelectList(list, "Id", "Name");
        }

        protected override void OnException(ExceptionContext filterContext) {
            if (filterContext.Exception != null) {
                LogException(filterContext.Exception);
            }

            base.OnException(filterContext);
        }

        private void LogException(Exception exc) {
            logging.Error(exc);
        }
    }
}
