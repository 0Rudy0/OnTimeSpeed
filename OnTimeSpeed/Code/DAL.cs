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

namespace OnTimeSpeed.Code
{
    public class DAL
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


        protected static string PostRequest(string endPoint, string token, Dictionary<string, string> formData)
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

        protected static string PostRequest(string endPoint, string token, object data)
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

        protected static string PostRequestAsync(string endPoint, string token, Dictionary<string, string> formData)
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

        protected static async Task<string> PostRequestAsync(string endPoint, string token, object data)
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

        public TokenData GetToken(string authCode, string returnUrl)
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

        public async Task<List<WorkLog>> GetWorkLogs(User user)
        {

            string cacheKey = "workLogs_" + user.id;
            var fromDate = DateTime.Now;
            fromDate.AddMonths(AppSettings.GetInt("monthsBack"));
            fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);

            var result = (List<WorkLog>)HttpRuntime.Cache.Get(cacheKey);
            if (result == null)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("start_date", fromDate.ToStringCustom());
                //parameters.Add("end_date", endDate?.AddDays(1).ToStringCustom());
                parameters.Add("assigned_to_id", user.id.ToString());

                var content = await GetRequestAsync($"api/v5/work_logs", user.Token, parameters);
                result = ApiHelper.GetObjectFromApiResponse<List<WorkLog>>(content);

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

        public async Task<List<WorkItem>> GetLunchTasks(User user)
        {
            string cacheKey = "lunchTasks";
            var result = (List<WorkItem>)HttpRuntime.Cache.Get(cacheKey);

            if (result == null)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("search_field", "[ID_NAME]");
                parameters.Add("search_string", "Pauza ručak");
                parameters.Add("columns", "status,id,item_type,name");

                var content = await GetRequestAsync($"api/v5/tasks", user.Token, parameters);
                var resultRaw = ApiHelper.GetObjectFromApiResponse<List<WorkItemRaw>>(content);

                result = new List<WorkItem>();
                resultRaw.ForEach(r =>
                {
                    result.Add(new WorkItem
                    {
                        Id = r.id,
                        Name = r.name,
                        Type = WorkItemType.Item
                    });
                });


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

        public async Task<bool> AddLunch(User user, DateTime fromDate, DateTime toDate)
        {
            var lunchTasks = await GetLunchTasks(user);
            var workLogs = await GetWorkLogs(user);

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                var lunchItem = PrepareData.GetLunchTaskForDate(lunchTasks, i);
                var logExists = PrepareData.DoesLogAlreadyExist(workLogs, lunchItem, i);
                if (logExists)
                    continue;
                else
                {
                    var newLog = PrepareData.CreateLunchWorkLogObj(user.id, lunchItem.Id, i);
                    var content = await PostRequestAsync($"/work_logs", user.Token, newLog);
                    var result = await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<WorkLog>(content));
                }
            }

            return true;
        }
    }
}