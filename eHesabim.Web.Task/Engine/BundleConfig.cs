using System.Web.Optimization;

namespace eHesabim.Web.Task.Engine {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            // jquery
            bundles.Add(
                new ScriptBundle("~/bundles/jquery")
                    .Include("~/Content/js/jquery-1.10.2.min.js")
                    .Include("~/Content/js/jquery-ui-1.10.3.min.js")
                    .Include("~/Content/js/jquery.blockUI.min.js")
                    .Include("~/Content/js/jquery.validate.min.js")
                    .Include("~/Content/js/jquery.validate.unobtrusive.min.js")
                    .Include("~/Content/js/jquery.unobtrusive-ajax.min.js"));
            bundles.Add(
                 new StyleBundle("~/bundles/jquery-ui")
                    .Include("~/Content/css/jquery-ui.css"));

            // kendo
            bundles.Add(
                new ScriptBundle("~/bundles/kendojs")
                    .Include("~/Content/js/kendo.web.min.js")
                    .Include("~/Content/js/kendo.all.min.js")
                    .Include("~/Content/js/kendo.aspnetmvc.min.js"));
            bundles.Add(
                new StyleBundle("~/bundles/kendocss")
                    .Include("~/Content/css/kendo.common.min.css")
                    .Include("~/Content/css/kendo.metro.min.css"));

            // scripts
            bundles.Add(
                 new ScriptBundle("~/bundles/scripts")
                     .Include("~/Content/js/scripts.js"));

            // style
            bundles.Add(
                 new StyleBundle("~/bundles/style")
                    .Include("~/Content/css/style.css"));
            
            BundleTable.EnableOptimizations = true;
        }
    }
}