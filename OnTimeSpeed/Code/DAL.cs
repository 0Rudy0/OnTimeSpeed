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
                result.ForEach(r => r.date_time.ToLocalTime());

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

        private static async Task<List<WorkItem>> GetWorkItems(
            User user,
            IEnumerable<string> searchStrings,
            List<string> itemTypes,
            string cacheKey)
        {
            var result = (List<WorkItem>)HttpRuntime.Cache.Get(cacheKey);

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
                            if (!result.Any(r => r.Id == rr.id))
                            {
                                result.Add(new WorkItem
                                {
                                    Id = rr.id,
                                    Name = rr.name,
                                    Type = WorkItemType.Item
                                });
                            }
                        });
                    }
                }

                HttpRuntime.Cache.Insert(cacheKey,
                    result,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(600),
                    CacheItemPriority.Normal,
                    null);
            }

            return result;
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

        #endregion


        #region Lunch

        public static async Task<List<WorkItem>> GetLunchTasks(User user)
        {
            string cacheKey = "lunchTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "rucakSearchString").SearchStrings;
            var result = await GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey);           

            return result;
        }
        

        public static async Task<List<string>> AddLunch(User user, DateTime fromDate, DateTime toDate)
        {
            var lunchTasks = await GetLunchTasks(user);
            var workLogs = await GetWorkLogs(user, true);
            var addedOnDates = new List<string> ();

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                var lunchItem = PrepareData.GetLunchTaskForDate(lunchTasks, i);
                var canAdd = PrepareData.CanAddWorkLog(workLogs, lunchItem, i);
                if (canAdd)
                {
                    var newLog = PrepareData.CreateLunchWorkLogObj(user.id, lunchItem.Id, i);
                    var content = await PostRequestAsync($"/work_logs", user.Token, newLog);
                    var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                    addedOnDates.Add(i.ToShortDateString());
                }
            }
            if (addedOnDates.Count > 0)
                HttpRuntime.Cache.Remove("workLogs_" + user.id);

            return addedOnDates;
        }

        #endregion

        #region Holiday

        public static async Task<List<WorkItem>> GetHolidayTasks(User user)
        {
            string cacheKey = "holidayTasks";
            var searchStrings = SearchStrings.Get().FirstOrDefault(s => s.Name == "holidaySearchString").SearchStrings;
            var result = await GetWorkItems(user, searchStrings, new List<string> { "tasks" }, cacheKey);

            return result;
        }


        public static async Task<List<string>> AddHolidays(User user, DateTime fromDate, DateTime toDate)
        {
            var tasks = await GetHolidayTasks(user);
            var workLogs = await GetWorkLogs(user, true);
            var addedOnDates = new List<string>();

            for (var i = fromDate; i <= toDate && i.IsHoliday(); i = i.AddDays(1))
            {
                var lunchItem = PrepareData.GetHolidayTaskForDate(tasks, i);
                var canAdd = PrepareData.CanAddWorkLog(workLogs, lunchItem, i, false);
                if (canAdd)
                {
                    var newLog = PrepareData.CreateLunchWorkLogObj(user.id, lunchItem.Id, i);
                    var content = await PostRequestAsync($"/work_logs", user.Token, newLog);
                    var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                    addedOnDates.Add(i.ToShortDateString());

                }
            }
            if (addedOnDates.Count > 0)
                HttpRuntime.Cache.Remove("workLogs_" + user.id);

            return addedOnDates;
        }

        #endregion
    }
}