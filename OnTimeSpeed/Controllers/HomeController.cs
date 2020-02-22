﻿using HrNetMobile.Models.Vacation;
using Newtonsoft.Json;
using OnTimeSpeed.Attributes;
using OnTimeSpeed.Code;
using OnTimeSpeed.Models;
using OnTimeSpeed.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using hrnet = HrNetMobile.Data;
using hrnetModel = HrNetMobile.Models;

namespace OnTimeSpeed.Controllers
{
    [AuthorizeOnTime]
    public class HomeController : Controller
    {
        private User _user;
        private int daysBack = 60;
        private hrnetModel.User _hrproUser;

        public HomeController()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("HR-hr");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("HR-hr");

            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["user"] != null)
                _user = (User)System.Web.HttpContext.Current.Session["user"];

            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["hrproUser"] != null)
                _hrproUser = (hrnetModel.User)System.Web.HttpContext.Current.Session["hrproUser"];

            daysBack = AppSettings.GetInt("daysBackToStartAdding") * -1;

            //if (DateUtils.Holidays.Count == 0)
            //    DateUtils.Holidays = DAL_HrProApi.GetHolidays();
        }

        public ActionResult Index()
        {
            var model = new MainModel();
            var user = User.Identity.Name;
            if (_hrproUser == null)
            {
                try
                {
                    _hrproUser = DAL.AuthHrNet(new hrnetModel.User
                    {
                        Username = user
                    });

                    model.HrProUser = _hrproUser;
                    Session["hrproUser"] = _hrproUser;
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                }
            }
            else
            {
                model.HrProUser = _hrproUser;
            }

            model.OnTimeUser = _user;
            if (_user != null)
                model.AllWorkTypes = DAL.GetWorkTypes();

            ViewBag.serial = VariousUtils.SerializeAndEncodeUser(_user);
            return View(model);
        }

        public async Task<bool> RecoverUser(string user)
        {
            if (String.IsNullOrEmpty(user))
            {
                return false;
            }
            else
            {
                try
                {
                    var userObj = VariousUtils.DecodeAndDeserializeUser(user);
                    if (userObj != null)
                    {
                        var ontimeUser = await DAL.GetMe(userObj);
                        if (ontimeUser != null)
                        {
                            Session["user"] = userObj;
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public ActionResult GetAuthCode(string code)
        {
            var returnUrl = string.Format("{0}://{1}{2}Home/GetAuthCode", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            var tokenData = DAL.GetToken(code, returnUrl);
            var user = new User
            {
                id = tokenData.data.id,
                name = tokenData.data.first_name + " " + tokenData.data.last_name,
                Token = tokenData.access_token
            };
            Session["user"] = user;
            Task.Run<List<WorkLog>>(async () => await DAL.GetWorkLogs(user, true));

            return RedirectToAction("Index");
        }

        public async Task<string> GetWorkLogs(string forDate, int? groupType)
        {
            var groupByForDateRange = groupType == null ? GroupBy.Month : (GroupBy)(groupType - 1);

            var groupBy = groupType == null ? GroupBy.Month : (GroupBy)(groupType);

            var dateRange = PrepareData.GetDateRange(forDate, groupByForDateRange);
            await DAL.GetLunchTasks(_user);
            var data = await DAL.GetWorkLogs(_user);
            return JsonConvert.SerializeObject(PrepareData.PrepareWorkLogChartData(data, groupBy, dateRange.Item1, dateRange.Item2));
        }

        public async Task<string> SearchWorkItems(string searchStr)
        {
            var items = await DAL.GetWorkItems(_user,
                new List<string> { searchStr },
                new List<string> { "items" }, null);

            return JsonConvert.SerializeObject(items);
        }

        #region Adding new

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddLunchToToday()
        {
            var addedOnDates = await DAL.AddLunch(
                _user,
                _hrproUser,
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBack") * -1).ToFirstOfMonth(), 
                DateTime.Now);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddHolidays()
        {
            var addedOnDates = await DAL.AddHolidays(
                _user, 
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBack") * -1).ToFirstOfMonth(), 
                DateTime.Now.ToLastOfMonth());

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddVacations()
        {
            var addedOnDates = await DAL.AddVacations(
                _user,
                _hrproUser,
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBack") * -1).ToFirstOfMonth(),
                DateTime.Now.ToLastOfMonth());

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddPaidLeaves()
        {
            var addedOnDates = await DAL.AddPaidLeave(
                _user,
                _hrproUser,
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBack") * -1).ToFirstOfMonth(),
                DateTime.Now.ToLastOfMonth());

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddCustom(
            int itemId,
            int workTypeId,
            float amount,
            string dateFromStr,
            string dateToStr,
            string itemType,
            string description)
        {
            var addedOnDates = await DAL.AddCustom(
                _user,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                itemId,
                workTypeId,
                amount,
                itemType,
                description);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        #endregion
    }
}