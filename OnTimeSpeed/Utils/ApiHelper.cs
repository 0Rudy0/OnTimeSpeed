using Newtonsoft.Json;
using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class ApiHelper
    {
        //private const string webApiTokenHeaderName = "Hrnet.Api.Token";
        //private const string mobileApiTokenHeaderName = "Hrnetmobile.Api.Token";


        public static T GetObjectFromApiResponse<T>(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                LogUtils.Debug("GetObjectFromApiResponse: Response is null or empty for unknown request");
                return default(T);
            }

            OnTimeResponseClass apiResult = null;
            try
            {
                apiResult = JsonConvert.DeserializeObject<OnTimeResponseClass>(response);
            }
            catch (Exception e)
            {
                LogUtils.LogException(e, null, null);
            }

            if (apiResult != null && apiResult.ErrorCode != null)
                LogUtils.LogException(new Exception(apiResult.ErrorCode), null, null);

            if (apiResult != null && apiResult.data != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(apiResult.data));
                }
                catch (Exception e)
                {
                    LogUtils.LogException(e, null, null);
                }
            }

            return default(T);
        }

        //public static string GetErrorFromApiResponse(string response)
        //{
        //    if (string.IsNullOrEmpty(response))
        //    {
        //        LogUtils.Debug("GetObjectFromApiResponse: Response is null or empty for unknown request");
        //        return null;
        //    }
        //    else
        //    {
        //        HrNetApiResult hrNetApiResult = null;
        //        try
        //        {
        //            hrNetApiResult = JsonConvert.DeserializeObject<HrNetApiResult>(response);
        //        }
        //        catch (Exception e)
        //        {
        //            LogException(e, null, null);
        //        }

        //        return hrNetApiResult?.ErrorMessage;
        //    }
        //}

        //public static HrNetApiResult GetApiResponse(string response)
        //{
        //    HrNetApiResult hrNetApiResult = null;
        //    try
        //    {
        //        hrNetApiResult = JsonConvert.DeserializeObject<HrNetApiResult>(response);
        //        return hrNetApiResult;
        //    }
        //    catch (Exception e)
        //    {
        //        LogException(e, null, null);
        //        return null;
        //    }
        //}

        #region GET

        public static string GetRequest(string url, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebRequest request = WebRequest.Create(url);

            request = WebRequest.Create(url);
            if (token != null)
                request.Headers.Add("Authorization", $"Bearer {token}");

            String wholeRequestString = "";

            //obtain a response
            var response = request.GetResponse();
            var objStream = response.GetResponseStream();
            var objReader = new StreamReader(objStream);

            //read line by line and construct a string response stream
            String sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    wholeRequestString += sLine;
            }
            return wholeRequestString;
        }

        public static async Task<string> GetRequestAsync(string url, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebRequest request = WebRequest.Create(url);
            string formedUrl = request.RequestUri.Scheme + "://" + request.RequestUri.Host + ":" + request.RequestUri.Port + "/";
            string[] segments = request.RequestUri.Segments;
            foreach (string tempSegment in segments)
            {
                formedUrl += tempSegment;
            }
            formedUrl += request.RequestUri.Query;
            request = WebRequest.Create(formedUrl);
            if (token != null)
                request.Headers.Add("Authorization", $"Bearer {token}");

            String wholeRequestString = "";

            //obtain a response
            var response = await request.GetResponseAsync();
            var objStream = response.GetResponseStream();
            var objReader = new StreamReader(objStream);

            //read line by line and construct a string response stream
            String sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    wholeRequestString += sLine;
            }
            return wholeRequestString;
        }

        #endregion

        #region POST

        //for WEB API - GET
        public static async Task<string> PostRequestAsync(string url, Dictionary<string, string> parameters, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();

                string postData = "";
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        postData += $"{param.Key}={param.Value}&";
                    }
                    postData = postData.Substring(0, postData.Length - 1); //remove last "&"
                    foreach (var param in parameters)
                        values[param.Key] = param.Value;
                }
                if (!String.IsNullOrEmpty(token))
                    client.Headers.Add("Authorization", $"Bearer {token}");

                var response = await client.UploadValuesTaskAsync(url, "POST", values);

                var responseString = Encoding.Default.GetString(response);

                return responseString;
            }
        }

        public static string PostRequest(string url, Dictionary<string, string> parameters, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();

                string postData = "";
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        postData += $"{param.Key}={param.Value}&";
                    }
                    postData = postData.Substring(0, postData.Length - 1); //remove last "&"
                    foreach (var param in parameters)
                        values[param.Key] = param.Value;
                }
                if (!String.IsNullOrEmpty(token))
                    client.Headers.Add("Authorization", $"Bearer {token}");

                var response = client.UploadValues(url, "POST", values);

                var responseString = Encoding.UTF8.GetString(response);

                return responseString;
            }
        }

        //for WEB API - POST
        public static async Task<string> PostRequestWithBodyAsync(string url, Dictionary<string, string> formData, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebRequest req = WebRequest.Create(url);
            string postData = "";
            foreach (var param in formData)
            {
                postData += $"{param.Key}={param.Value}&";
            }
            postData = postData.Substring(0, postData.Length - 1); //remove last "&"

            byte[] send = Encoding.Default.GetBytes(postData);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;
            req.Headers.Add("Authorization", $"Bearer {token}");

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = await req.GetResponseAsync();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            return returnvalue;
        }

        public static string PostRequestWithBody(string url, Dictionary<string, string> formData, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebRequest req = WebRequest.Create(url);
            string postData = "";
            foreach (var param in formData)
            {
                postData += $"{param.Key}={param.Value}&";
            }
            postData = postData.Substring(0, postData.Length - 1); //remove last "&"

            byte[] send = Encoding.Default.GetBytes(postData);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;
            req.Headers.Add("Authorization", $"Bearer {token}");
            req.Timeout = 15000;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            return returnvalue;
        }
        //for WEB API - POST
        public static async Task<string> PostRequestWithBodyAsync(string url, object data, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(data), new UTF8Encoding(), "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //if (!String.IsNullOrEmpty(token))
            //{
            //    request.Content.Headers.Add("Authorization", $"Bearer {token}");
            //}


            var httpClient = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 30)
            };
            if (!String.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var result = "";
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await httpClient.SendAsync(request);
                result = await response.Content.ReadAsStringAsync();
                //result = ApiHelper.GetErrorFromApiResponse await Task.Factory.StartNew(() => ApiHelper.GetObjectFromApiResponse<string>(content));

                response.EnsureSuccessStatusCode();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string PostRequestWithBody(string url, object data, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(data), new UTF8Encoding(), "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (!String.IsNullOrEmpty(token))
                request.Content.Headers.Add("Authorization", $"Bearer {token}");

            var httpClient = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 30)
            };
            var response = httpClient.SendAsync(request).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        #endregion
    }
}