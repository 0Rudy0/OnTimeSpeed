using HrNetMobile.Models.Vacation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using hrnetModel = HrNetMobile.Models;
using hrnet = HrNetMobile.Data;
using System.Web.Caching;

namespace OnTimeSpeed.Code
{
    public static class DAL_HrProApi
    {
        private static hrnet.DAL.VacationAPI _vacationAPI = new hrnet.DAL.VacationAPI();

        public static async Task<Dictionary<DateTime, string>> GetHolidays(hrnetModel.User hrproUser)
        {
            string cacheKey = "holidaysHrProAPI";
            Dictionary<DateTime, string> holidays = (Dictionary<DateTime, string>)HttpRuntime.Cache.Get(cacheKey);
            if (holidays == null && hrproUser != null)
            {
                holidays = new Dictionary<DateTime, string>();

                //holidays.AddRange((await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year - 1, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi)).Select(h => h.DateFrom).ToList());
                var result1 = await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year - 1, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi);

                var dict1 = result1.ToDictionary(x => x.DateFrom, y => y.Name);
                var dict2 = (await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi)).ToDictionary(x => x.DateFrom, y => y.Name);
                var dict3 = (await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year + 1, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi)).ToDictionary(x => x.DateFrom, y => y.Name);

                foreach (var d in dict1)
                    if (holidays.ContainsKey(d.Key) == false)
                        holidays.Add(d.Key, d.Value);

                foreach (var d in dict2)
                    if (holidays.ContainsKey(d.Key) == false)
                        holidays.Add(d.Key, d.Value);
                foreach (var d in dict3)

                    if (holidays.ContainsKey(d.Key) == false)
                        holidays.Add(d.Key, d.Value);

                HttpRuntime.Cache.Insert(cacheKey,
                    holidays,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(1440), //1 dan
                    CacheItemPriority.Normal,
                    null);
            }

            return holidays;
        }

        public static async Task<Dictionary<DateTime, string>> GetApprovedVacationDays(hrnetModel.User hrproUser)
        {
            var cacheKey = "vacations_" + hrproUser.Username + DateTime.Now.Ticks;

            var vacationDays = (Dictionary<DateTime, string>)HttpRuntime.Cache.Get(cacheKey);
            if (vacationDays == null && hrproUser != null)
            {
                var userId = hrproUser.LoggedContact.ID;
                var token = hrproUser.TokenWebApi;
                vacationDays = new Dictionary<DateTime, string>();

                var vacationRequests = new List<VacationRequest>();
                var vacationRequests1 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year - 1, userId, userId, false, token);
                var vacationRequests2 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year, userId, userId, false, token);
                var vacationRequests3 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year + 1, userId, userId, false, token);
                var temp = new List<VacationRequest>();
                temp.AddRange(vacationRequests1);
                temp.AddRange(vacationRequests2);
                temp.AddRange(vacationRequests3);

                vacationRequests.AddRange(temp.Where(v => v.RequestType == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestType.Vacation &&
                    v.RequestStatus == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestStatus.VacationApproved));

                foreach (var req in vacationRequests)
                {
                    for (var i = req.DateFrom; i <= req.DateTo; i = i.AddDays(1))
                    {
                        if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && !vacationDays.ContainsKey(i))
                            vacationDays.Add(i, "GO");
                    }
                }

                HttpRuntime.Cache.Insert(cacheKey,
                    vacationDays,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(5),
                    CacheItemPriority.Normal,
                    null);
            }

            return vacationDays;
        }

        public static async Task<Dictionary<DateTime, string>> GetPaidLeaves(hrnetModel.User hrproUser)
        {
            var cacheKey = "paidLeaves_" + hrproUser.Username;
            var paidLeaves = (Dictionary<DateTime, string>)HttpRuntime.Cache.Get(cacheKey);

            if (paidLeaves == null && hrproUser != null)
            {
                var userId = hrproUser.LoggedContact.ID;
                var token = hrproUser.TokenWebApi;
                paidLeaves = new Dictionary<DateTime, string>();

                var requests = new List<VacationRequest>();
                var requests1 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year - 1, userId, userId, false, token);
                var requests2 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year, userId, userId, false, token);
                var requests3 = await _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year + 1, userId, userId, false, token);
                var temp = new List<VacationRequest>();
                temp.AddRange(requests1);
                temp.AddRange(requests2);
                temp.AddRange(requests3);

                requests.AddRange(temp.Where(v => v.RequestType == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestType.PaidLeave &&
                v.RequestStatus == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestStatus.PaidLeaveApproved));

                foreach (var req in requests)
                {
                    for (var i = req.DateFrom; i <= req.DateTo; i = i.AddDays(1))
                    {
                        if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && !paidLeaves.ContainsKey(i))
                            paidLeaves.Add(i, req.RequestTypeName ?? "Plaćeni dopust");
                    }
                }

                HttpRuntime.Cache.Insert(cacheKey,
                    paidLeaves,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(30),
                    CacheItemPriority.Normal,
                    null);
            }

            return paidLeaves;
        }
    }
}