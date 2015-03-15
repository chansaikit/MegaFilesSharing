using System.Web;
using System.Web.Optimization;

namespace MegaFilesSharing
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.js" 
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       // "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-ui-1.10.4.js" ,
                        "~/Scripts/icheck.js" 
                        ));

            bundles.Add(new ScriptBundle("~/bundles/IonTabs").Include(
                        "~/Scripts/ion.tabs.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*")); 

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css",
                        "~/Content/Loginbtn.css",
                        "~/Content/Pushbtn.css",
                        "~/Content/Div.css",
                        "~/Content/PagedList.css",
                        "~/Content/ion.tabs.css",
                        "~/Content/ion.tabs.skinBordered.css",
                        "~/Content/normalize.css",
                        "~/Content/styles.css",
                        "~/Content/blue.css"
                        ));


             
             

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery-ui.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"
                        ));

            BundleTable.EnableOptimizations = true;
        }
    }
}