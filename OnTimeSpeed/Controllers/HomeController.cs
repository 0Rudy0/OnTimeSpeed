using HrNetMobile.Models.Vacation;
using Newtonsoft.Json;
using OnTimeSpeed.Attributes;
using OnTimeSpeed.Code;
using OnTimeSpeed.EntryImplementations;
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

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            
        }

        public ActionResult Index()
        {
            var model = new MainModel();
            var user = User.Identity.Name;
            if (_hrproUser == null)
            {
                try
                {
                    LogUtils.Debug("Pokušaj logina s " + user);
                    _hrproUser = DAL.AuthHrNet(new hrnetModel.User
                    {
                        Username = user
                    });
                    if (_hrproUser == null)
                        LogUtils.Debug("neuspješna prijava");
                    else
                        LogUtils.Debug("Prijava uspješna!");

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
            if (_user != null) 
                HttpRuntime.Cache.Remove("workLogs_" + _user.id);
            //_user = new User
            //{
            //    id = 5,
            //    name = "Danijel Rudman",
            //    Token = "bla"
            //};

            model.OnTimeUser = _user;
            if (_user != null)
                model.AllWorkTypes = DAL.GetWorkTypes();         

            ViewBag.serial = VariousUtils.SerializeAndEncodeUser(_user);
            return View(model);
        }

        public async Task<string> GetHolidays()
        {
            if (DateUtils.Holidays.Count == 0 && _hrproUser != null)
                DateUtils.Holidays = await DAL_HrProApi.GetHolidays(_hrproUser);
            else
            {
                try
                {
                    DateUtils.Holidays = Holidays.GetHolidays(DateTime.Now.Year);
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                }
            }
            return JsonConvert.SerializeObject(DateUtils.Holidays);
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
            var data = await DAL.GetWorkLogs(_user);
            return JsonConvert.SerializeObject(PrepareData.PrepareWorkLogChartData(data, groupBy, dateRange.Item1, dateRange.Item2));
        }

        [HttpPost]
        public async Task<string> DeleteWorkLogs(string forDate, int? groupType)
        {
            var groupByForDateRange = groupType == null ? GroupBy.Month : (GroupBy)(groupType - 1);
            var dateRange = PrepareData.GetDateRange(forDate, groupByForDateRange);
            var data = await DAL.GetWorkLogs(_user);
            var toDelete = data.Where(l => l.date_time.Date >= dateRange.Item1 && l.date_time.Date <= dateRange.Item2);
            var success = await DAL.DeleteWorkLogs(_user, toDelete);

            HttpRuntime.Cache.Remove("workLogs_" + _user.id);

            return JsonConvert.SerializeObject(new
            {
                logsCount = toDelete.Count(),
                workAmount = toDelete.Sum(l => l.work_done.duration_minutes / 60),
                fromDateStr = dateRange.Item1.ToStringCustom(),
                toDateStr = dateRange.Item2.Value.ToStringCustom()
            });
        }

        public async Task<string> DeleteWorkLog(int logId)
        {
            var success = await DAL.DeleteWorkLogs(_user, new List<WorkLog> { new WorkLog { id = logId } });
            return JsonConvert.SerializeObject(success);
        }

        public async Task<string> UpdateWorkLog(
            int logId,
            string description,
            string amount)
        {
            float.TryParse(amount, System.Globalization.NumberStyles.AllowDecimalPoint, new System.Globalization.CultureInfo("en"), out var amount1);
            float.TryParse(amount, System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out var amount2);
            var amountParsed = amount1 > amount2 && amount1 > 0 ? amount1 : amount2;

            var success = await DAL.UpdateWorkLog(_user, logId, description, amountParsed);
            return JsonConvert.SerializeObject(success);
        }

        public async Task<string> DeleteWorkLogsValidate(string forDate, int? groupType)
        {
            var groupByForDateRange = groupType == null ? GroupBy.Month : (GroupBy)(groupType - 1);
            var dateRange = PrepareData.GetDateRange(forDate, groupByForDateRange);
            var data = await DAL.GetWorkLogs(_user);
            var toDelete = data.Where(l => l.date_time.Date >= dateRange.Item1 && l.date_time.Date <= dateRange.Item2);

            return JsonConvert.SerializeObject(new {
                logsCount = toDelete.Count(),
                workAmount = toDelete.Sum(l => l.work_done.duration_minutes / 60),
                fromDateStr = dateRange.Item1.ToStringCustom(),
                toDateStr = dateRange.Item2.Value.ToStringCustom()
            });
        }

        public async Task<string> SearchWorkItems(string searchStr)
        {
            var items = await DAL.GetWorkItems(_user,
                new List<string> { searchStr },
                new List<string> { "items" }, null);

            return JsonConvert.SerializeObject(items);
        }

        #region Adding new

        #region Add automatic

        [HttpPost]
        //[AuthorizeHrPro]
        public async Task<string> AddAllAutomatic(string amount = "0.5")
        {
            var addedOnDates = JsonConvert.DeserializeObject<List<string>>(await AddHolidays(true));
            addedOnDates.AddRange(JsonConvert.DeserializeObject<List<string>>(await AddVacations(true)));
            addedOnDates.AddRange(JsonConvert.DeserializeObject<List<string>>(await AddPaidLeaves(true)));
            addedOnDates.AddRange(JsonConvert.DeserializeObject<List<string>>(await AddLunchToToday(amount, true)));

            if (addedOnDates.Count > 0 && addedOnDates.ElementAt(addedOnDates.Count - 1).Contains("-------"))
                addedOnDates.RemoveAt(addedOnDates.Count - 1); //remove last separator

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        //[AuthorizeHrPro]
        public async Task<string> AddLunchToToday(
            string amount, bool detailedLog = false)
        {
            float.TryParse(amount, System.Globalization.NumberStyles.AllowDecimalPoint, new System.Globalization.CultureInfo("en"), out var amount1);
            float.TryParse(amount, System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out var amount2);
            var amountParsed = amount1 > amount2 && amount1 > 0 ? amount1 : amount2;

            var addedOnDates = await DAL.AddAutomatic(
                new LunchEntry(),
                _user,
                _hrproUser,
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBackToStartAdding") * -1).ToFirstOfMonth(),
                DateTime.Now,
                detailedLog,
                amountParsed);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddHolidays(bool detailedLog = false)
        {
            var addedOnDates = await DAL.AddAutomatic(
                new HolidayEntry(),
                _user,
                _hrproUser,
                DateTime.Now.AddMonths(AppSettings.GetInt("monthsBackToStartAdding") * -1).ToFirstOfMonth(),
                DateTime.Now.ToLastOfMonth(),
                detailedLog);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddVacations(bool detailedLog = false)
        {
            var addedOnDates = await DAL.AddAutomatic(
               new VacationEntry(),
               _user,
               _hrproUser,
               DateTime.Now.AddMonths(AppSettings.GetInt("monthsBackToStartAdding") * -1).ToFirstOfMonth(),
               DateTime.Now.ToLastOfMonth(),
               detailedLog);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddPaidLeaves(bool detailedLog = false)
        {
            var addedOnDates = await DAL.AddAutomatic(
              new PaidLeaveEntry(),
              _user,
              _hrproUser,
              DateTime.Now.AddMonths(AppSettings.GetInt("monthsBackToStartAdding") * -1).ToFirstOfMonth(),
              DateTime.Now.ToLastOfMonth(),
              detailedLog);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        #endregion

        #region Add semi automatic

        [HttpPost]
        //[AuthorizeHrPro]
        public async Task<string> AddLunch(
           float amount,
           string dateFromStr,
           string dateToStr,
           string description,
           bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new LunchEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddSickLeave(
            float amount,
            string dateFromStr,
            string dateToStr,
            string description,
            bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new SickLeaveEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddInternalMeeting(
            float amount,
            string dateFromStr,
            string dateToStr,
            string description,
            bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new InternalMeetingEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddOnTimeEntry(
            float amount,
            string dateFromStr,
            string dateToStr,
            string description,
            bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new OnTimeEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddColegueSupport(
            float amount,
            string dateFromStr,
            string dateToStr,
            string description,
            bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new ColleagueSupportEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddEducation(
            float amount,
            string dateFromStr,
            string dateToStr,
            string description,
            bool ignoreFullDays)
        {
            var addedOnDates = await DAL.AddSemiAutomatic(
                new EducationEntry(),
                _user,
                _hrproUser,
                dateFromStr.ToDate(),
                String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                amount,
                description,
                ignoreFullDays);

            return JsonConvert.SerializeObject(addedOnDates);
        }

        #endregion

        [HttpPost]
        [AuthorizeHrPro]
        public async Task<string> AddCustom(
            int itemId,
            int workTypeId,
            float amount,
            string dateFromStr,
            string dateToStr,
            string itemType,
            string description,
            bool ignoreFullDays)
        {
            if (amount > 0)
            {
                var addedOnDates = await DAL.AddCustom(
                    _user,
                    dateFromStr.ToDate(),
                    String.IsNullOrEmpty(dateToStr) ? dateFromStr.ToDate() : dateToStr.ToDate(),
                    itemId,
                    workTypeId,
                    amount,
                    itemType,
                    description,
                    ignoreFullDays);

                return JsonConvert.SerializeObject(addedOnDates);
            }
            else
            {
                return "Iznos radnih sati je 0";
            }
        }

        #endregion
    }
}