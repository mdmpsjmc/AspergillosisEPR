#region Using

using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

#endregion

namespace AspergillosisEPR.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static string _displayVersion;
        private static Assembly _executingAssembly;
        private static Settings _settings;

        public static Settings Settings => _settings;

        public static void EnsureSettings(this IHtmlHelper helper)
        {
            _settings = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IOptions<Settings>>().Value;
        }

        public static IHtmlContent AssemblyVersion(this IHtmlHelper helper)
        {
            if (_displayVersion.IsEmpty())
                SetDisplayVersion();

            return helper.Raw(_displayVersion);
        }

        public static IHtmlContent RouteIf(this IHtmlHelper helper, string value, string attribute)
        {
            var currentController = (helper.ViewContext.RouteData.Values["controller"] ?? string.Empty).ToString().UnDash();
            var currentAction = (helper.ViewContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();

            var hasController = value.Equals(currentController, StringComparison.OrdinalIgnoreCase);
            var hasAction = value.Equals(currentAction, StringComparison.OrdinalIgnoreCase);

            return hasAction || hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }

        public static void RenderPartialIf(this IHtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
                return;

            htmlHelper.RenderPartialAsync(partialViewName).GetAwaiter().GetResult();
        }

        public static IHtmlContent Copyright(this IHtmlHelper helper)
        {
            helper.EnsureSettings();

            var copyright = $"{helper.AssemblyVersion()} &copy; {DateTime.Now.Year} {Settings.Company}".Trim();

            return helper.Raw(copyright);
        }

        private static void SetDisplayVersion()
        {
            _executingAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var version = _executingAssembly.GetName().Version;

            _displayVersion =
                string.Format("{4} {0}.{1}.{2} (build {3})", version.Major, version.Minor, version.Build,
                    version.Revision, Settings.Project).Trim();
        }

        /// <summary>
        ///     Returns an unordered list (ul element) of validation messages that utilizes bootstrap markup and styling.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="alertType">The alert type styling rule to apply to the summary element.</param>
        /// <param name="heading">The optional value for the heading of the summary element.</param>
        /// <returns></returns>
        public static IHtmlContent ValidationBootstrap(this IHtmlHelper htmlHelper, string alertType = "danger",
            string heading = "")
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
                return new HtmlString(string.Empty);

            var div = new TagBuilder("div");
            div.AddCssClass($"alert alert-{alertType} alert-block");

            var button = new TagBuilder("button");
            button.AddCssClass("close");
            button.Attributes.Add("data-dismiss", "alert");
            button.Attributes.Add("aria-hidden", "true");
            button.InnerHtml.AppendHtml("&times;");

            div.InnerHtml.AppendHtml(button);

            if (!heading.IsEmpty())
            {
                var h4 = new TagBuilder("h4");
                h4.AddCssClass("alert-heading");
                h4.InnerHtml.Append($"<h4 class=\"alert-heading\">{heading}</h4>");

                div.InnerHtml.AppendHtml(h4);
            }

            var summary = htmlHelper.ValidationSummary();

            div.InnerHtml.AppendHtml(summary);

            return div;
        }
    }
}