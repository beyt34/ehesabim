using System;
using System.Linq;
using System.Web.Mvc;

using eHesabim.Core.Engine;
using eHesabim.Framework.Localization;

namespace eHesabim.Framework {
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel> {
        private IWorkContext workContext;

        protected IWorkContext WorkContext {
            get { return workContext; }
        }

        public static string ValidationSummary(ModelStateDictionary modelErrors, string validationSummaryMessage, bool useBreak = true) {
            var errors = (from v in modelErrors.Values where v.Errors.Count > 0 select v.Errors).ToList();
            if (errors.Any()) {
                var returnValue = string.Format("{0} <br/>", validationSummaryMessage);
                return errors.Aggregate(returnValue, (current, error) => current + string.Format("<span>{0}</span>{1}", error[0].ErrorMessage, useBreak ? "<br/>" : string.Empty));
            }

            return string.Empty;
        }

        public override void InitHelpers() {
            base.InitHelpers();
            workContext = EngineContext.Current.Resolve<IWorkContext>();
        }

        protected string GetPageHeader(int id, string addTitleLabel, string editTitleLabel, string editTitleValue) {
            return string.Format("{0} {1}", id == 0 ? addTitleLabel : string.Format("{0} {1} {2}", editTitleLabel, string.IsNullOrEmpty(editTitleValue) ? string.Empty : "-", editTitleValue), string.Format("<a id='close' class='button round blue image-right ic-cancel text-upper' href='#' onclick='CloseWindow();'>{0}</a>", Buttons.Close));
        }

        protected string GetPageHeaderRefresh(int id, string addTitleLabel, string editTitleLabel, string editTitleValue) {
            return string.Format("{0} {1}", id == 0 ? addTitleLabel : string.Format("{0} {1} {2}", editTitleLabel, string.IsNullOrEmpty(editTitleValue) ? string.Empty : "-", editTitleValue), string.Format("<a id='close' class='btn btn-default btn-circle pull-right' href='#' onclick='RefreshOpener();'><i class='fa fa-times'></i></a><br/><br/>"));
        }

        protected string GetPageHeader(Guid id, string addTitleLabel, string editTitleLabel, string editTitleValue) {
            return string.Format("{0} {1}", id == Guid.Empty ? addTitleLabel : string.Format("{0} {1} {2}", editTitleLabel, string.IsNullOrEmpty(editTitleValue) ? string.Empty : "-", editTitleValue), string.Format("<a id='close' class='button round blue image-right ic-cancel text-upper' href='#' onclick='CloseWindow();'>{0}</a>", Buttons.Close));
        }

        protected string GetPageHeaderRefresh(Guid id, string addTitleLabel, string editTitleLabel, string editTitleValue) {
            return string.Format("{0} {1}", id == Guid.Empty ? addTitleLabel : string.Format("{0} {1} {2}", editTitleLabel, string.IsNullOrEmpty(editTitleValue) ? string.Empty : "-", editTitleValue), string.Format("<a id='close' class='btn btn-default btn-circle pull-right' href='#' onclick='RefreshOpener();'><i class='fa fa-times'></i></a><br/><br/>"));
        }
    }
}