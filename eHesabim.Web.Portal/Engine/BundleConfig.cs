using System.Web.Optimization;
using eHesabim.Web.Portal.Helper;

namespace eHesabim.Web.Portal.Engine {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new StyleBundle(CssBundles.JQueryUi).Include("~/Content/css/jquery-ui.min.css"));
            bundles.Add(new StyleBundle(CssBundles.Bootstrap).Include(
                "~/Content/css/bootstrap.min.css", 
                "~/Content/css/bootstrap-social.css", 
                "~/Content/font-awesome/css/font-awesome.min.css"));

            bundles.Add(new StyleBundle(CssBundles.SbAdmin).Include("~/Content/css/sb-admin.css"));
            bundles.Add(new StyleBundle(CssBundles.Kendo).Include("~/Content/css/kendo.common.min.css", "~/Content/css/kendo.metro.min.css"));
            bundles.Add(new StyleBundle(CssBundles.Landing).Include("~/Content/css/landing.css"));

            bundles.Add(
                new ScriptBundle(JsBundles.JQuery).Include(
                    "~/Content/js/jquery-1.10.2.min.js", 
                    "~/Content/js/jquery.unobtrusive-ajax.min.js",
                    "~/Content/js/jquery.validate.min.js",
                    "~/Content/js/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle(JsBundles.JQueryUi).Include("~/Content/js/jquery-ui-1.10.3.min.js", "~/Content/js/jquery.blockUI.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.JQueryMenu).Include("~/Content/js/jquery.metisMenu.js"));
            bundles.Add(new ScriptBundle(JsBundles.Bootstrap).Include("~/Content/js/bootstrap.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.SbAdmin).Include("~/Content/js/sb-admin.js"));
            bundles.Add(new ScriptBundle(JsBundles.SbAdminNew).Include("~/Content/js/sb-admin-2.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.Kendo).Include(
                "~/Content/js/kendo.all.min.js",
                "~/Content/js/kendo.aspnetmvc.min.js", 
                "~/Content/js/kendo.web.min.js", 
                "~/Content/js/kendo.culture.tr-TR.min.js"));

            // new design
            bundles.Add(new StyleBundle(CssBundles.SbAdminNew).Include("~/Content/css/sb-admin-2.min.css"));
            bundles.Add(new StyleBundle(CssBundles.Style).Include("~/Content/css/style.min.css"));

            bundles.Add(new ScriptBundle(JsBundles.JQueryNew).Include("~/Content/vendor/jquery/jquery.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.JQueryEasingNew).Include("~/Content/vendor/jquery-easing/jquery.easing.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.BootstrapNew).Include("~/Content/vendor/bootstrap/js/bootstrap.bundle.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.SbAdminNew).Include("~/Content/js/sb-admin-2.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.Scripts).Include("~/Content/js/scripts.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}