using Newtonsoft.Json;
using OnTimeSpeed.Code;
using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnTimeSpeed.Controllers
{
    public class HomeController : Controller
    {
        private DAL _dal;
        private User _user;

        public HomeController()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("HR-hr");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("HR-hr");

            _dal = new DAL();
            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["user"] != null)
                _user = (User)System.Web.HttpContext.Current.Session["user"];
        }

        public ActionResult Index()
        {           
            return View(_user);
        }

        public ActionResult GetAuthCode(string code)
        {
            var returnUrl = string.Format("{0}://{1}{2}Home/GetAuthCode", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            var tokenData = _dal.GetToken(code, returnUrl);
            var user = new User
            {
                id = tokenData.data.id,
                name = tokenData.data.first_name + " " + tokenData.data.last_name,
                Token = tokenData.access_token
            };
            Session["user"] = user;

            return RedirectToAction("Index");
        }

        public async Task<string> GetWorkLogs(string forDate, int? groupType)
        {
            var groupByForDateRange = groupType == null ? GroupBy.Month : (GroupBy)(groupType - 1);
            var groupBy = groupType == null ? GroupBy.Month : (GroupBy)(groupType);

            var dateRange = PrepareData.GetDateRange(forDate, groupByForDateRange);
            await _dal.GetLunchTasks(_user);
            var data = await _dal.GetWorkLogs(_user);
            await _dal.AddLunch(_user, DateTime.Now.AddDays(-2), DateTime.Now);
            return JsonConvert.SerializeObject(PrepareData.PrepareWorkLogChartData(data, groupBy, dateRange.Item1, dateRange.Item2));
        }
    }
}