using Newtonsoft.Json;
using OnTimeSpeed.Models;
using OnTimeSpeed.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using hrnet = HrNetMobile.Data;
using hrnetModel = HrNetMobile.Models;
using hrnetHelper = HrNetMobile.Common;
using HrNetMobile.Models.Vacation;
using OnTimeSpeed.EntryImplementations;

namespace OnTimeSpeed.Code
{
    public static class DAL
    {
        static string _apiUrl = AppSettings.Get("ontimeApiUrl");
        static string _ontimeUrl = AppSettings.Get("ontimeUrl");
        static string _clientId = AppSettings.Get("clientId");
        static string _clientSecret = AppSettings.Get("clientSecret");

        #region PRIVATE - RequestHelpers

        private static string GetRequest(string endPoint, string token = null, Dictionary<string, string> parameters = null)
        {
            string paramsStr = "";
            if (parameters != null)
            {
                paramsStr = "?";
                foreach (var param in parameters)
                {
                    paramsStr += $"{param.Key}={HttpUtility.UrlEncode(param.Value)}&";
                }
                paramsStr = paramsStr.Substring(0, paramsStr.Length - 1); //remove last "&"
            }

            try
            {
                var responseString = ApiHelper.GetRequest($"{_ontimeUrl}/{endPoint}{paramsStr}", token);

                //if (responseString.Contains("Token does not exist."))
                //{
                //    throw new SessionExpiredException("Session expired");
                //}
                return responseString;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The remote server returned an error: (403) Forbidden"))
                    LogUtils.Debug(ex, null, null, "url: " + $"{_apiUrl}{endPoint}{paramsStr}");
                else
                    LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}{paramsStr}");
                throw;
            }
        }

        private static async Task<string> GetRequestAsync(string endPoint, string token = null, Dictionary<string, string> parameters = null)
        {
            string paramsStr = "";
            if (parameters != null)
            {
                paramsStr = "?";
                foreach (var param in parameters)
                {
                    paramsStr += $"{param.Key}={HttpUtility.UrlEncode(param.Value)}&";
                }
                paramsStr = paramsStr.Substring(0, paramsStr.Length - 1); //remove last "&"
            }

            try
            {
                var responseString = await ApiHelper.GetRequestAsync($"{_ontimeUrl}/{endPoint}{paramsStr}", token);

                //if (responseString.Contains("Token does not exist."))
                //{
                //    throw new SessionExpiredException("Session expired");
                //}
                return responseString;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The remote server returned an error: (403) Forbidden"))
                    LogUtils.Debug(ex, null, null, "url: " + $"{_apiUrl}{endPoint}{paramsStr}");
                else
                    LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}{paramsStr}");
                throw;
            }
        }


        private static string PostRequest(string endPoint, string token, Dictionary<string, string> formData)
        {
            try
            {
                return ApiHelper.PostRequestWithBody($"{_apiUrl}{endPoint}", formData, token);
            }
            catch (WebException ex)
            {
                var data = "";
                foreach (var d in formData)
                {
                    data += $"{d.Key}={d.Value} ; ";
                }
                LogUtils.LogException(ex, null, null, "endpoint: " + $"{_apiUrl}{endPoint}-----formData:{data}");
                //var stream = ex.Response.GetResponseStream();
                //var reader = new StreamReader(stream);
                //var content = reader.ReadToEnd();
                //var exception = JsonConvert.DeserializeObject<Error>(content);
                return null;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex, null, null, "url: " + endPoint);
                return null;
            }
        }

        private static string PostRequest(string endPoint, string token, object data)
        {
            try
            {
                return ApiHelper.PostRequestWithBody($"{_apiUrl}{endPoint}", data, token);
            }
            catch (WebException ex)
            {
                var stream = ex.Response.GetResponseStream();
                var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                //var exception = JsonConvert.DeserializeObject<Error>(content);
                LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}");
                return null;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}");
                return null;
            }
        }

        private static string PostRequestAsync(string endPoint, string token, Dictionary<string, string> formData)
        {
            try
            {
                return ApiHelper.PostRequestWithBody($"{_apiUrl}{endPoint}", formData, token);
            }
            catch (WebException ex)
            {
                var stream = ex.Response.GetResponseStream();
                var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                //var exception = JsonConvert.DeserializeObject<Error>(content);
                LogUtils.LogException(new Exception(content), null, null, "url: " + $"{_apiUrl}{endPoint}");
                return null;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}");
                return null;
            }
        }

        private static async Task<string> PostRequestAsync(string endPoint, string token, object data)
        {
            try
            {
                return await ApiHelper.PostRequestWithBodyAsync($"{_apiUrl}{endPoint}", data, token);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex, null, null, "url: " + $"{_apiUrl}{endPoint}");
                return null;
            }
        }

        #endregion

        public static TokenData GetToken(string authCode, string returnUrl)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("code", authCode);
            parameters.Add("redirect_uri", returnUrl);
            parameters.Add("client_id", _clientId);
            parameters.Add("client_secret", _clientSecret);

            var content = GetRequest($"api/oauth2/token", null, parameters);
            var result = JsonConvert.DeserializeObject<TokenData>(content);

            return result;
        }

        public static async Task<List<WorkLog>> GetWorkLogs(User user, bool forceRefresh = false)
        {

            string cacheKey = "workLogs_" + user.id;
            var fromDate = DateTime.Now;
            fromDate = fromDate.AddMonths(AppSettings.GetInt("monthsBack") * -1);
            fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);

            var result = (List<WorkLog>)HttpRuntime.Cache.Get(cacheKey);
            if (result == null || forceRefresh)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("start_date", fromDate.ToStringCustom());
                //parameters.Add("end_date", endDate?.AddDays(1).ToStringCustom());
                parameters.Add("assigned_to_id", user.id.ToString());

                var content = await GetRequestAsync($"api/v5/work_logs", user.Token, parameters);
                result = ApiHelper.GetObjectFromApiResponse<List<WorkLog>>(content);

                result = result.OrderBy(r => r.date_time).ToList();
                result.ForEach(r =>
                {
                    r.date_time.ToLocalTime();
                    r.description = String.IsNullOrEmpty(r.description) ? String.Empty : r.description;
                });

                HttpRuntime.Cache.Insert(cacheKey,
                    result,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(30),
                    CacheItemPriority.Normal,
                    null);
            }

            return result;
        }

        public static async Task<Dictionary<DateTime, float>> GetWorkLogsGroupped(User user, bool forceRefresh = false)
        {
            string cacheKey = "workLogsGroupped_" + user.id;

            var logs = await GetWorkLogs(user, forceRefresh);

            var groupped = (Dictionary<DateTime, float>)HttpRuntime.Cache.Get(cacheKey);
            if (groupped == null)
            {
                groupped = logs.GroupBy(g => g.date_time.Date).
                    ToDictionary(g => g.Key, g => g.Sum(l => l.work_done.duration_minutes / 60));
            }

            return groupped;
        }

        public static async Task<List<WorkItem>> GetWorkItems(
            User user,
            IEnumerable<string> searchStrings,
            List<string> itemTypes,
            string cacheKey = null)
        {
            List<WorkItem> result = null;

            if (!String.IsNullOrEmpty(cacheKey))
                result = (List<WorkItem>)HttpRuntime.Cache.Get(cacheKey);

            if (result == null)
            {
                foreach (var type in itemTypes)
                {
                    foreach (var str in searchStrings)
                    {
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("search_field", "[ID_NAME]");
                        parameters.Add("search_string", str);
                        parameters.Add("columns", "status,id,item_type,name");

                        var content = await GetRequestAsync($"api/v5/{type}", user.Token, parameters);
                        var resultRaw = ApiHelper.GetObjectFromApiResponse<List<WorkItemRaw>>(content);

                        result = new List<WorkItem>();
                        resultRaw.ForEach(rr =>
                        {
                            if (!result.Any(r => r.Id == rr.id && r.TypeString == rr.item_type))
                            {
                                result.Add(new WorkItem
                                {
                                    Id = rr.id,
                                    Name = rr.name,
                                    Type = WorkItemType.Item,
                                    TypeString = rr.item_type
                                });
                            }
                        });
                    }
                }

                if (!String.IsNullOrEmpty(cacheKey))
                {
                    HttpRuntime.Cache.Insert(cacheKey,
                        result,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(600),
                        CacheItemPriority.Normal,
                        null);
                }
            }

            return result;
        }

        public static async Task<List<OnTimeUser>> GetUser(string searchString, User user)
        {
            var cacheKey = "allHrProOnTimeUsers";

            var allUsers = (List<OnTimeUser>)HttpRuntime.Cache.Get(cacheKey);

            if (allUsers == null)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("include_inactive", "false");

                var content = await GetRequestAsync($"api/v5/users", user.Token, parameters);
                allUsers = ApiHelper.GetObjectFromApiResponse<List<OnTimeUser>>(content);
            }

            HttpRuntime.Cache.Insert(cacheKey,
                allUsers,
                null,
                Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(600),
                CacheItemPriority.Normal,
                null);


            return allUsers.Where(u => $"{u.first_name.ToLower()} {u.last_name.ToLower()}".Contains(searchString.Trim().ToLower()) ||
                $"{u.last_name.ToLower()} {u.first_name.ToLower()}".Contains(searchString.Trim().ToLower())).ToList();
        }

        public static List<Work_Log_Type> GetWorkTypes()
        {
            var cacheKey = "workLogTypes";
            var allTypes = (List<Work_Log_Type>)HttpRuntime.Cache.Get(cacheKey);
            if (allTypes == null)
            {
                var content = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/config/workTypes.json");
                allTypes = JsonConvert.DeserializeObject<List<Work_Log_Type>>(content);

                HttpRuntime.Cache.Insert(cacheKey,
                    allTypes,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(99999),
                    CacheItemPriority.Normal,
                    null);
            }

            return allTypes;
        }


        #region AUTH

        public static hrnetModel.User AuthHrNet(hrnetModel.User user)
        {
            var token = hrnet.DAL.DALWebApi.AuthenticateWebApiWin(user);
            user.TokenWebApi = token.ApiToken;
            user.LoggedContact = new hrnetModel.Contact
            {
                Name = token.EmployeeName,
                Email = token.EmployeeEmail,
                ID = token.EmployeeId
            };

            //try
            //{
            //    user.LoggedContact.ProfilePicture = hrnetHelper.HelpFunctions.GetProfilePicture(user.LoggedContact.ID, 100, user.TokenWebApi);

            //}
            //catch
            //{
            //    user.LoggedContact.ProfilePictureFallback = appAbsoluteUrl + "/Content/images/personPlaceholder.png";
            //}

            return user;
        }

        public static async Task<OnTimeUser> GetMe(User user)
        {
            var parameters = new Dictionary<string, string>();
            //parameters.Add("search_field", "[ID_NAME]");
            //parameters.Add("search_string", str);
            //parameters.Add("columns", "status,id,item_type,name");

            var content = await GetRequestAsync($"api/v5/me", user.Token, parameters);
            var result = ApiHelper.GetObjectFromApiResponse<OnTimeUser>(content);

            return result;
        }

        #endregion

        #region ADD WORK ITEMS

        public static async Task<List<string>> AddAutomatic(
            IAutomaticEntry entry,
            User user,
            hrnetModel.User hrproUser,
            DateTime fromDate,
            DateTime toDate,
            bool detailedLog = false,
            float lunchAmount = 0)
        {
            var tasks = await entry.GetAllRelatedTasks(user);
            var workLogs = await GetWorkLogs(user, true);
            var vacations = await entry.GetApprovedVacationDays(hrproUser);
            var addedOnDates = new List<string>();

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                var workItem = entry.GetTaskForDate(tasks, i);
                var canAdd = entry.CanAddWorkLog(workLogs, vacations, workItem, i, lunchAmount, out float addAmount);
                if (canAdd)
                {
                    var newLog = entry.CreateWorkLogObj(user.id, workItem.Id, i, addAmount, vacations);
                    var content = await PostRequestAsync($"/work_logs", user.Token, newLog);
                    var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                    if (detailedLog)
                        addedOnDates.Add($"{i.ToShortDateString()} - {entry.GetEntryDescription()}");
                    else
                        addedOnDates.Add(i.ToShortDateString());
                }
            }
            if (addedOnDates.Count > 0)
            {
                HttpRuntime.Cache.Remove("workLogs_" + user.id);

                if (detailedLog)
                    addedOnDates.Add("----------------------"); //separator
            }


            return addedOnDates;
        }

        public static async Task<List<string>> AddSemiAutomatic(
            ISemiAutomaticEntry entry,
            User user,
            hrnetModel.User hrproUser,
            DateTime? fromDate,
            DateTime? toDate,
            float amount,
            string description,
            bool ignoreFullDays)
        {
            if (fromDate == null || toDate == null)
                throw new Exception("Ne postoji datumski raspon za unos");

            var tasks = await entry.GetAllRelatedTasks(user);

            var addedOnDates = new List<string>();

            var workLogs = await GetWorkLogsGroupped(user, true);

            for (var i = fromDate; i <= toDate; i = i.Value.AddDays(1))
            {
                var alreadyLogedAmount = 0f;
                if (workLogs.ContainsKey(i.Value.Date))
                    alreadyLogedAmount = workLogs[i.Value.Date];

                if (!i.Value.IsHoliday() && !i.Value.IsWeekend() && (alreadyLogedAmount < 8 || ignoreFullDays))
                {
                    var amountToLog = amount > 8 ? 8 - alreadyLogedAmount : amount;
                    if (amountToLog > 0)
                    {
                        var workItem = entry.GetTaskForDate(tasks, i.Value);
                        var newLog = entry.CreateWorkLogObj(user.id, workItem.Id, i.Value, amountToLog, description);
                        var content = await PostRequestAsync($"/work_logs", user.Token, newLog);
                        if (content == null)
                            addedOnDates.Add($"Unos nije uspio - {i.Value.ToShortDateString()}");
                        else
                        {
                            var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                            var warningMsg = "";
                            if (alreadyLogedAmount + amountToLog > 8)
                                warningMsg = " *** više od 8h zalograno ***";
                            addedOnDates.Add(i.Value.ToShortDateString() + warningMsg);
                        }
                    }
                    else
                    {
                        addedOnDates.Add($"Dan je već popunjen - {i.Value.ToShortDateString()}");
                    }
                }
                else if (alreadyLogedAmount >= 8)
                {
                    addedOnDates.Add($"Dan je već popunjen - {i.Value.ToShortDateString()}");
                }
            }
            if (addedOnDates.Count > 0)
                HttpRuntime.Cache.Remove("workLogs_" + user.id);

            return addedOnDates;
        }

        public static async Task<List<string>> AddCustom(User user, DateTime? fromDate, DateTime? toDate,
            int itemId, int workTypeId, float amount, string itemType, string description, bool ignoreFullDays)
        {
            var addedOnDates = new List<string>();

            if (fromDate == null || toDate == null)
                throw new Exception("Ne postoji datumski raspon za unos");

            var workLogs = await GetWorkLogsGroupped(user, true);

            for (var i = fromDate; i <= toDate; i = i.Value.AddDays(1))
            {
                var alreadyLogedAmount = 0f;
                if (workLogs.ContainsKey(i.Value.Date))
                    alreadyLogedAmount = workLogs[i.Value.Date];

                if (!i.Value.IsHoliday() && !i.Value.IsWeekend() && (alreadyLogedAmount < 8 || ignoreFullDays))
                {
                    var amountToLog = amount > 8 ? 8 - alreadyLogedAmount : amount;
                    if (amountToLog > 0)
                    {
                        var newWorkLog = PrepareData.CreateWorkLogObject(user.id, amountToLog, workTypeId, itemId, itemType, i.Value, description);
                        var content = await PostRequestAsync($"/work_logs", user.Token, newWorkLog);
                        if (content == null)
                            throw new Exception("Unos nije uspio");
                        var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                        var warningMsg = "";
                        if (alreadyLogedAmount + amountToLog > 8)
                            warningMsg = " *** više od 8h zalograno ***";
                        addedOnDates.Add(i.Value.ToShortDateString() + warningMsg);
                    }
                    else
                    {
                        addedOnDates.Add($"Dan je već popunjen - {i.Value.ToShortDateString()}");
                    }
                }
                else if (alreadyLogedAmount >= 8)
                {
                    addedOnDates.Add($"Dan je već popunjen - {i.Value.ToShortDateString()}");
                }
            }
            if (addedOnDates.Count > 0)
                HttpRuntime.Cache.Remove("workLogs_" + user.id);

            return addedOnDates;
        }

        #endregion

        #region NO USE FOR NOW 

        //ne koristi se nigdje aktivno, po potrebi će se iskoristiti za dohvat svih work typeova iz postojećih logova
        public static List<Work_Log_Type> GetWorkTypesFromResponse(User user)
        {

            var content = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/config/response.json");
            var result = ApiHelper.GetObjectFromApiResponse<List<WorkLog>>(content);
            var allTypes = result.Select(l => l.work_log_type).Where(wl => !String.IsNullOrEmpty(wl.name) && !wl.name.ToLower().Contains("ne koristiti"))
                .GroupBy(wl => wl.id).Select(group => group.First()).OrderBy(gg => gg.name).ToList();

            return allTypes;
        }

        #endregion
    }

}