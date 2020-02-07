﻿using OnTimeSpeed.Utils;
using System.Web;
using System.Web.Optimization;

namespace OnTimeSpeed
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/Layout").Include(
                      "~/Scripts/materialize.min.js",
                      "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/Index").Include(
                      "~/Scripts/highcharts/highcharts.js",
                      "~/Scripts/highcharts/themes/sand-signika.js",
                      "~/Scripts/index.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/Layout").Include(
                      //"~/Content/ontime1.css",
                      //"~/Content/ontime2.css",
                      //"~/Content/ontime3.css",
                      "~/Content/materialize.min.css",
                      "~/Content/site.css")
                .Include("~/Content/FontAwesome/all.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/materializeIcons.css", new CssRewriteUrlTransformWrapper()));

            bundles.Add(new StyleBundle("~/Content/Index").Include(
                      "~/Content/highcharts/highcharts.css"));
        }
    }
}
