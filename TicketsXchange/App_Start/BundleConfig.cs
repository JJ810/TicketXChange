using System.Web;
using System.Web.Optimization;

namespace TicketsXchange
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/js").Include(
                        "~/Content/js/jquery-3.2.0.min.js",
                        "~/Content/js/bootstrap-slider.min.js",
                        "~/Content/js/bootstrap-select.min.js",
                        "~/Content/js/jquery.scrolling-tabs.min.js",
                        "~/Content/js/jquery.countdown.min.js",
                        "~/Content/js/jquery.flexslider-min.js",
                        "~/Content/js/jquery.imagemapster.min.js",
                        "~/Content/js/jquery.autocomplete.min.js",
                        "~/Content/js/handlebars.js",
                        "~/Content/js/tooltip.js",
                        "~/Content/js/bootstrap.min.js",
                        "~/Content/js/featherlight.min.js",
                        "~/Content/js/featherlight.gallery.min.js",
                        "~/Content/js/bootstrap.offcanvas.min.js",
                        "~/Content/js/datepicker.min.js",
                        "~/Content/js/datepicker.en.js",
                        "~/Content/js/typehead.js",
                        "~/Content/js/validator.min.js",
                        "~/Content/js/main.js",
                        "~/Content/js/login_modal.js"
                        ));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-select.min.css",
                      "~/Content/css/bootstrap-slider.min.css",
                      "~/Content/css/jquery.scrolling-tabs.min.css",
                      "~/Content/css/bootstrap-checkbox.css",
                      "~/Content/css/flexslider.css",
                      "~/Content/css/featherlight.min.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/bootstrap.offcanvas.min.css",
                      "~/Content/css/datepicker.min.css",
                      "~/Content/css/core.css",
                      "~/Content/css/style.css",
                      "~/Content/css/login_modal.css",
                      "~/Content/css/responsive.css"
                      ));

        }
    }
}
