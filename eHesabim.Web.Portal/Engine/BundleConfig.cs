using System.Web.Optimization;
using eHesabim.Web.Portal.Helper;

namespace eHesabim.Web.Portal.Engine {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            // css
            bundles.Add(new StyleBundle(CssBundles.Bootstrap).Include("~/Content/vendor/bootstrap/bootstrap.min.css"));
            bundles.Add(new StyleBundle(CssBundles.Kendo).Include("~/Content/vendor/kendo/kendo.common.css", "~/Content/vendor/kendo/kendo.metro.css"));
            bundles.Add(new StyleBundle(CssBundles.Landing).Include("~/Content/css/landing.css"));
            bundles.Add(new StyleBundle(CssBundles.SbAdmin).Include("~/Content/css/sb-admin-2.css"));
            bundles.Add(new StyleBundle(CssBundles.Style).Include("~/Content/css/style.css"));

            // js
            bundles.Add(new ScriptBundle(JsBundles.JQuery).Include("~/Content/vendor/jquery/jquery.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.JQueryEasing).Include("~/Content/vendor/jquery/jquery.easing.min.js"));

            bundles.Add(new ScriptBundle(JsBundles.JQueryValidation).Include(
                "~/Content/vendor/jquery/jquery.validate.min.js",
                "~/Content/vendor/jquery/jquery.validate.unobtrusive.min.js",
                "~/Content/vendor/jquery/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle(JsBundles.Kendo).Include(
                "~/Content/vendor/kendo/kendo.all.min.js",
                "~/Content/vendor/kendo/kendo.aspnetmvc.min.js",
                "~/Content/vendor/kendo/kendo.web.min.js",
                "~/Content/vendor/kendo/kendo.culture.tr-TR.min.js"));

            bundles.Add(new ScriptBundle(JsBundles.Bootstrap).Include("~/Content/vendor/bootstrap/bootstrap.min.js"));
            bundles.Add(new ScriptBundle(JsBundles.SbAdmin).Include("~/Content/js/sb-admin-2.js"));
            bundles.Add(new ScriptBundle(JsBundles.Scripts).Include("~/Content/js/scripts.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}