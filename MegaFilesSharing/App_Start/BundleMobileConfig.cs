using System.Web;
using System.Web.Optimization;

namespace MegaFilesSharing {
    public class BundleMobileConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquerymobile")
                .Include(
                "~/Scripts/jquery.mobile-1.4.2.js"));

            bundles.Add(new StyleBundle("~/Content/Mobile/css")
                .Include("~/Content/Site.Mobile.css"));
            
            bundles.Add(new StyleBundle("~/Content/jquerymobile/css")
                .Include(
                //"~/Content/jquery.mobile-{version}.css",
                "~/Content/JQueryMobile/jquery.mobile-1.4.2.css", 
                "~/Content/JQueryMobile/jquery.mobile.external-png-1.4.2.css",  
                "~/Content/JQueryMobile/jquery.mobile.icons-1.4.2.css", 
                "~/Content/JQueryMobile/jquery.mobile.inline-png-1.4.2.css", 
                "~/Content/JQueryMobile/jquery.mobile.inline-svg-1.4.2.css", 
                "~/Content/JQueryMobile/jquery.mobile.structure-1.4.2.css", 
                "~/Content/JQueryMobile/jquery.mobile.theme-1.4.2.css"
                ));

            BundleTable.EnableOptimizations = true;
        }
    }
}