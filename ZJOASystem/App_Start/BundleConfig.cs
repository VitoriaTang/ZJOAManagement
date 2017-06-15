using System.Web;
using System.Web.Optimization;

namespace ZJOASystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/bootstrap/js/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/css/bootstrap.min.css",
                      "~/Content/bootstrap/css/bootstrap-responsive.css",
                      "~/Content/template/styles.css",
                      "~/Content/jqwidgets/styles/jqx.base.css",
                      "~/Content/jqwidgets/styles/jqx.darkblue.css"));

            bundles.Add(new ScriptBundle("~/bundles/zjoa").Include(
                        "~/Scripts/zjoa.js",
                        "~/Scripts/custom.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqwidgets/treegrid").Include(
                        "~/Content/jqwidgets/jqxcore.js",
                        "~/Content/jqwidgets/jqxdata.js",
                        "~/Content/jqwidgets/jqxbuttons.js",
                        "~/Content/jqwidgets/jqxscrollbar.js",
                        "~/Content/jqwidgets/jqxlistbox.js",
                        "~/Content/jqwidgets/jqxdropdownlist.js",
                        "~/Content/jqwidgets/jqxdatatable.js",
                        "~/Content/jqwidgets/jqxtreegrid.js",
                        "~/Content/jqwidgets/jqxtooltip.js",
                        "~/Content/jqwidgets/jqxinput.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqwidgets/dialog").Include(
                        "~/Content/jqwidgets/jqxcore.js",
                        "~/Content/jqwidgets/jqxdata.js",
                        "~/Content/jqwidgets/jqxbuttons.js",
                        "~/Content/jqwidgets/jqxlistbox.js",
                        "~/Content/jqwidgets/jqxdropdownlist.js",
                        "~/Content/jqwidgets/jqxtooltip.js",
                        "~/Content/jqwidgets/jqxinput.js",
                        "~/Content/jqwidgets/jqxfileupload.js",
                        "~/Content/jqwidgets/jqxwindow.js",
                        "~/Content/jqwidgets/jqxscrollbar.js",
                        "~/Content/jqwidgets/jqxpanel.js",
                        "~/Content/jqwidgets/jqxmenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqwidgets/editor").Include(
                        "~/Content/jqwidgets/jqxcore.js",
                        "~/Content/jqwidgets/jqxdata.js",
                        "~/Content/jqwidgets/jqxbuttons.js",
                        "~/Content/jqwidgets/jqxlistbox.js",
                        "~/Content/jqwidgets/jqxscrollbar.js",
                        "~/Content/jqwidgets/jqxcombobox.js",
                        "~/Content/jqwidgets/jqxdatetimeinput.js",
                        "~/Content/jqwidgets/jqxcalendar.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqwidgets/grid").Include(
                        "~/Content/jqwidgets/jqxcore.js",
                        "~/Content/jqwidgets/jqxdata.js",
                        "~/Content/jqwidgets/jqxbuttons.js",
                        "~/Content/jqwidgets/jqxlistbox.js",
                        "~/Content/jqwidgets/jqxscrollbar.js",
                        "~/Content/jqwidgets/jqxcombobox.js",
                        "~/Content/jqwidgets/jqxdropdownlist.js",
                        "~/Content/jqwidgets/jqxgrid.js",
                        "~/Content/jqwidgets/jqxgrid.sort.js",
                        "~/Content/jqwidgets/jqxgrid.pager.js",
                        "~/Content/jqwidgets/jqxgrid.selection.js"));

        }
    }
}
