using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnTimeSpeed.Attributes
{
    public class AuthorizeAttr : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var action = actionContext.RouteData.Values["action"].ToString();
            var type = actionContext.Controller.GetType();

            var isActionResult = false;
            if (action == "RecoverUser")
            {
                isActionResult = true;
            }
            else
            {
                var method = type.GetMethod(action);
                if (method == null && !string.IsNullOrEmpty(action))
                {
                    action = action.First().ToString().ToUpper() + action.Substring(1);
                    method = type.GetMethod(action);
                }
                var returnType = method?.ReturnType;
                isActionResult = returnType == typeof(ActionResult) || returnType == typeof(Task<ActionResult>);
            }

            if (!isActionResult && (actionContext.HttpContext.Session["hrproUser"] == null ||
                    actionContext.HttpContext.Session["user"] == null))
            {
                actionContext.HttpContext.Response.AddHeader("bla", DateTime.Now.Ticks.ToString());
                //throw new Exception("Login required");
            }
            else
            {
                actionContext.HttpContext.Response.AddHeader("bla-bla", DateTime.Now.Ticks.ToString());
            }

        }

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);

        //    var action = filterContext.RouteData.Values["action"].ToString();
        //    var type = filterContext.Controller.GetType();

        //    var isActionResult = false;
        //    if (action == "RecoverUser")
        //    {
        //        isActionResult = true;
        //    }
        //    else
        //    {
        //        var method = type.GetMethod(action);
        //        if (method == null && !string.IsNullOrEmpty(action))
        //        {
        //            action = action.First().ToString().ToUpper() + action.Substring(1);
        //            method = type.GetMethod(action);
        //        }
        //        var returnType = method?.ReturnType;
        //        isActionResult = returnType == typeof(ActionResult) || returnType == typeof(Task<ActionResult>);
        //    }

        //    if (!isActionResult && (filterContext.HttpContext.Session["hrproUser"] == null ||
        //            filterContext.HttpContext.Session["user"] == null))
        //    {                
        //        filterContext.HttpContext.Response.AddHeader("bla", DateTime.Now.Ticks.ToString()); 
        //        //throw new Exception("Login required");
        //    }
        //    else
        //    {
        //        filterContext.HttpContext.Response.AddHeader("bla-bla", DateTime.Now.Ticks.ToString()); 
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class AuthorizeOnTime : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            base.OnAuthorization(actionContext);

            var action = actionContext.RouteData.Values["action"].ToString();
            var type = actionContext.Controller.GetType();

            var isActionResult = false;
            if (action == "RecoverUser")
            {
                isActionResult = true;
            }
            else
            {
                var method = type.GetMethod(action);
                if (method == null && !string.IsNullOrEmpty(action))
                {
                    action = action.First().ToString().ToUpper() + action.Substring(1);
                    method = type.GetMethod(action);
                }
                var returnType = method?.ReturnType;
                isActionResult = returnType == typeof(ActionResult) || returnType == typeof(Task<ActionResult>);
            }

            if (!isActionResult && actionContext.HttpContext.Session["user"] == null)
            {
                actionContext.HttpContext.Response.AddHeader("REQUIRES_AUTH_ONTIME", "1");
                actionContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            else
            {
                actionContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class AuthorizeHrPro : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            base.OnAuthorization(actionContext);

            var action = actionContext.RouteData.Values["action"].ToString();
            var type = actionContext.Controller.GetType();

            var isActionResult = false;
            if (action == "RecoverUser")
            {
                isActionResult = true;
            }
            else
            {
                var method = type.GetMethod(action);
                if (method == null && !string.IsNullOrEmpty(action))
                {
                    action = action.First().ToString().ToUpper() + action.Substring(1);
                    method = type.GetMethod(action);
                }
                var returnType = method?.ReturnType;
                isActionResult = returnType == typeof(ActionResult) || returnType == typeof(Task<ActionResult>);
            }

            if (!isActionResult && actionContext.HttpContext.Session["hrproUser"] == null)
            {
                actionContext.HttpContext.Response.AddHeader("REQUIRES_AUTH_HRPRO", "1");
                actionContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
    }
}