using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class LogUtils
    {

        public static void LogError(string data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\t<div class='exception card card-danger'>");
            sb.AppendLine("\t\t<div class='card-header'>");
            sb.AppendLine("\t\t\t<h3 class='exception card-title'> ERROR </h3>");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t\t<div class='card-body'>");
            sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
            sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Message:</span>\t<code>" + (data ?? "NO DATA") + "</code></span><br />");
            sb.AppendLine("\t\t\t<span class=\"title\">StackTrace:</span><br />");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t</div>");

            FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
        }

        public static void LogException(Exception e, object token = null, object userId = null, string additionalInfo = "")
        {
            if (token == null)
            {
                token = "unknown";
            }
            if (userId == null)
            {
                userId = "-1";
            }
            var sb = new StringBuilder();
            sb.AppendLine("\t<div class='exception card card-danger'>");
            sb.AppendLine("\t\t<div class='card-header'>");
            sb.AppendLine("\t\t\t<h3 class='exception card-title'> EXCEPTION </h3>");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t\t<div class='card-body'>");
            sb.AppendLine(String.Format("\t\t\t<h5 class='loggedUser'>Used token: <strong>{0}</strong> ({1})</h5>", token.ToString(), userId.ToString()));
            sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
            if (e != null)
            {
                sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Message:</span>\t<code>" + e.Message + "</code></span><br />");
                sb.AppendLine("\t\t\t<span class=\"exceptionSource\"><span class=\"title\">Source:</span>\t<code>" + e.Source + "</code></span><br />");
                sb.AppendLine("\t\t\t<span class=\"exceptionTargetSite\"><span class=\"title\">TargetSite:</span>\t<code>" + e.TargetSite + "</code></span><br />");
            }
            if (additionalInfo != null && additionalInfo.Contains("url:"))
            {
                string link = additionalInfo.Split(' ')[1];
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Url:</span>\t<code><a href=" + link + " target='_blank'>" + link + "</a></code></span><br />");
            }
            else
            {
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Additional info:</span>\t<code>" + additionalInfo + "</code></span><br />");
            }
            sb.AppendLine("\t\t\t<span class=\"title\">StackTrace:</span><br />");
            sb.AppendLine("\t\t\t<code class=\"exceptionStacktrace\">" + e != null && e.StackTrace != null ? e.StackTrace.Replace(" at", "<br>at") : "Exception object or Stack Trace is null" + "</code><br />");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t</div>");

            FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
        }

        public static void LogException(Error e, object token = null, object userId = null, string additionalInfo = "")
        {
            if (token == null)
            {
                token = "unknown";
            }
            if (userId == null)
            {
                userId = "-1";
            }
            var sb = new StringBuilder();
            sb.AppendLine("\t<div class='exception card card-danger'>");
            sb.AppendLine("\t\t<div class='card-header'>");
            sb.AppendLine("\t\t\t<h3 class='exception card-title'> EXCEPTION </h3>");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t\t<div class='card-body'>");
            sb.AppendLine(String.Format("\t\t\t<h5 class='loggedUser'>Used token: <strong>{0}</strong> ({1})</h5>", token.ToString(), userId.ToString()));
            sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
            if (e != null)
            {
                sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Message:</span>\t<code>" + e.Message + "</code></span><br />");
            }
            if (additionalInfo != null && additionalInfo.Contains("url:"))
            {
                string link = additionalInfo.Split(' ')[1];
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Url:</span>\t<code><a href=" + link + " target='_blank'>" + link + "</a></code></span><br />");
            }
            else
            {
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Additional info:</span>\t<code>" + additionalInfo + "</code></span><br />");
            }
            sb.AppendLine("\t\t\t<span class=\"title\">StackTrace:</span><br />");
            sb.AppendLine("\t\t\t<code class=\"exceptionStacktrace\">" + e != null && e.StackTrace != null ? e.StackTrace.Replace(" at", "<br>at") : "Exception object or Stack Trace is null" + "</code><br />");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t</div>");

            FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
        }

        public static void Debug(Exception e, object token = null, object userId = null, string additionalInfo = "")
        {
            if (token == null)
            {
                token = "unknown";
            }
            if (userId == null)
            {
                userId = "-1";
            }
            var sb = new StringBuilder();
            sb.AppendLine("\t<div class='debug card card-primary'>");
            sb.AppendLine("\t\t<div class='card-header'>");
            sb.AppendLine("\t\t\t<h3 class='debug card-title'> DEBUG (EXCEPTION) </h3>");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t\t<div class='card-body'>");
            sb.AppendLine(String.Format("\t\t\t<h5 class='loggedUser'>Used token: <strong>{0}</strong> ({1})</h5>", token.ToString(), userId.ToString()));
            sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
            sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Message:</span>\t<code>" + e.Message + "</code></span><br />");
            sb.AppendLine("\t\t\t<span class=\"exceptionSource\"><span class=\"title\">Source:</span>\t<code>" + e.Source + "</code></span><br />");
            sb.AppendLine("\t\t\t<span class=\"exceptionTargetSite\"><span class=\"title\">TargetSite:</span>\t<code>" + e.TargetSite + "</code></span><br />");
            if (additionalInfo != null && additionalInfo.Contains("url:"))
            {
                string link = additionalInfo.Split(' ')[1];
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Url:</span>\t<code><a href=" + link + " target='_blank'>" + link + "</a></code></span><br />");
            }
            else
            {
                sb.AppendLine("\t\t\t<span class=\"classTest\"><span class=\"title\">Additional info:</span>\t<code>" + additionalInfo + "</code></span><br />");
            }
            sb.AppendLine("\t\t\t<span class=\"title\">StackTrace:</span><br />");
            sb.AppendLine("\t\t\t<code class=\"exceptionStacktrace\">" + e != null && e.StackTrace != null ? e.StackTrace.Replace(" at", "<br>at") : "Exception object or Stack Trace is null" + "</code><br />");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t</div>");

            FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
        }



        public static void LogClientErrors(List<JsLogEventCollection> jsLogEventCollections, string username)
        {
            var sb = new StringBuilder();

            foreach (JsLogEventCollection collection in jsLogEventCollections)
            {
                sb.AppendLine("\t<div class='exception card card-danger'>");
                sb.AppendLine("\t\t<div class='card-header'>");
                sb.AppendLine("\t\t\t<h3 class='exception card-title'>ERROR</h3>");
                sb.AppendLine("\t\t</div>");
                sb.AppendLine("\t\t<div class='card-body'>");

                sb.AppendLine(String.Format("<h5 class='loggedUser'>Logged user: <strong>{0}</strong></h5>", username));
                var currPage = collection.ElementAt(0).Page;

                sb.AppendLine($"<strong>Steps on page <code>{currPage}</code>:</strong>");
                sb.AppendLine("<ol>");
                foreach (JsLogEvent logEvent in collection)
                {
                    if (logEvent.Page != currPage)
                    {
                        sb.AppendLine("</ol>");
                        currPage = logEvent.Page;
                        sb.AppendLine($"<strong>Steps on page <code>{currPage}</code>:</strong>");
                        sb.AppendLine("<ol>");
                    }
                    sb.AppendLine("<li>");
                    sb.AppendLine("<span class='timeUtc' style='display: none'>" + DateTime.UtcNow.ToString("o") + "</span>");
                    switch (logEvent.Title)
                    {
                        case "User Click":
                            sb.AppendLine(String.Format("{0} - User click on element: <code>{1}</code>", logEvent.DateActual, logEvent.Element.Replace("<", "&lt;").Replace(">", "&gt;")));
                            break;
                        case "XMLHttpRequest":
                            sb.AppendLine(String.Format("{0} - Ajax call to: <code>{1}</code>. Response time: {2}, response status: {3}", logEvent.DateActual, logEvent.Url, logEvent.ResponseDuration, logEvent.ResponseStatus));
                            break;
                        case "Error":
                            sb.AppendLine(String.Format("{0} - An error occurred: <code>{1}</code>. <br/>Stack trace<br /><code class=\"exceptionStacktrace\">{2}</code>", logEvent.DateActual, logEvent.ErrorMessage, logEvent.Stack != null ? logEvent.Stack.Replace(" at", "<br>at").Replace("@", "<br>at ") : ""));
                            break;
                    }
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ol>");
                sb.AppendLine("\t\t</div>");
                sb.AppendLine("\t</div>");
            }

            FileUtils.WriteToLog(sb.ToString(), "ClientLog.html");
        }

        public static void Debug(string msg, string token = "", string userId = "")
        {
            if (System.Web.HttpContext.Current?.IsDebuggingEnabled == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("\t<div class='debug card card-primary'>");
                sb.AppendLine("\t\t<div class='card-header'>");
                sb.AppendLine("\t\t\t<h3 class='debug card-title'> DEBUG </h3>");
                sb.AppendLine("\t\t</div>");
                sb.AppendLine("\t\t<div class='card-body'>");
                sb.AppendLine(String.Format("\t\t\t<h5 class='loggedUser'>Used token: <strong>{0}</strong> ({1})</h5>", token, userId));
                sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
                sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Message:</span>\t<code>" + System.Web.HttpUtility.HtmlEncode(msg) + "</code></span><br />");
                sb.AppendLine("\t\t</div>");
                sb.AppendLine("\t</div>");

                FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
            }
        }

        public static void Data(String json, string token = "", string userId = "")
        {
            if (!System.Web.HttpContext.Current.IsDebuggingEnabled) return;

            json = json.Replace("{", "<br/>");
            json = json.Replace("}", "<br/>");
            json = json.Replace(",", "<br/>");
            json = json.Replace('"', ' ');

            var sb = new StringBuilder();
            sb.AppendLine("\t<div class='data card card-success'>");
            sb.AppendLine("\t\t<div class='card-header'>");
            sb.AppendLine("\t\t\t<h3 class='data card-title'> DATA </h3>");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t\t<div class='card-body'>");
            sb.AppendLine(String.Format("\t\t\t<h5 class='loggedUser'>Used token: <strong>{0}</strong> ({1})</h5>", token, userId));
            sb.AppendLine("\t\t\t<span class=\"exceptionTime\"><span class=\"title\">Time: </span><span>" + DateTime.Now + "</span><span class='timeUtc' style='display: none'>" + DateTime.Now.ToString("o") + "</span> <br />");
            sb.AppendLine("\t\t\t<span class=\"exceptionMessage\"><span class=\"title\">Object data:</span><br />\t<code>" + json + "</code></span><br />");
            sb.AppendLine("\t\t</div>");
            sb.AppendLine("\t</div>");

            FileUtils.WriteToLog(sb.ToString(), "ServerLog.html");
        }
    }
}