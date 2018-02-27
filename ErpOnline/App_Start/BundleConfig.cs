using System.Web;
using System.Web.Optimization;

namespace ErpOnline
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bootstrap
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/bootstrap.min.css"));
            // Bootstrap
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/assets/css/bootstrap.css",
                      "~/Content/assets/css/custom.css",
                      "~/Content/bootstrap-select.min.css"));
            // Font Awsome
            bundles.Add(new StyleBundle("~/Content/aws").Include(
                      "~/Content/assets/css/font-awesome.css",
                      "~/Content/SweetAlert/sweetalert2.min.css"));
            // Data tables
            bundles.Add(new StyleBundle("~/Content/datatble").Include(
                      "~/Content/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/datatables.extensions/Responsive/css/dataTables.responsive.css",
                      "~/Content/DataTables/css/dataTables.bootstrap.min.css",
                      "~/Content/DataTables/css/responsive.dataTables.min.css"));

            //------------------------------------------------------------------------------------------------
            
            // Jquery script
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/assets/js/jquery-1.10.2.js"));
            // jquery validation script
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            // modernizr script
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bootstrap script
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Content/assets/js/bootstrap.min.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootstrap-select.min.js"));
            // sweet alert script
            bundles.Add(new ScriptBundle("~/bundles/swt").Include(
                        "~/Scripts/SweetAlert/sweetalert2.min.js"));
            // my script
            bundles.Add(new ScriptBundle("~/bundles/erpsc").Include(
                        "~/Content/assets/js/jquery.metisMenu.js",
                        "~/Content/assets/js/custom.js",
                        "~/Scripts/layoutsrc.js"));
            // data table script
            bundles.Add(new ScriptBundle("~/bundles/dtbjs").Include(
                        "~/Scripts/CDN.jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js"));
        }
    }
}
