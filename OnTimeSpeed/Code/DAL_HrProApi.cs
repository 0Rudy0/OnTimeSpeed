﻿using HrNetMobile.Models.Vacation;
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

        public static List<DateTime> GetHolidays(hrnetModel.User hrproUser)
        {
            string cacheKey = "holidays";
            List<DateTime> holidays = (List<DateTime>)HttpRuntime.Cache.Get(cacheKey);
            if (holidays == null)
            {
                holidays = new List<DateTime>();

                var holidays1Task = Task.Run<List<Holiday>>(async () => await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year - 1, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi));
                var holidays2Task = Task.Run<List<Holiday>>(async () => await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi));
                var holidays3Task = Task.Run<List<Holiday>>(async () => await _vacationAPI.GetHolidaysAsync(DateTime.Now.Year + 1, hrproUser.LoggedContact.ID, hrproUser.TokenWebApi));
                holidays.AddRange(holidays1Task.Result.Select(h => h.DateFrom).ToList());
                holidays.AddRange(holidays2Task.Result.Select(h => h.DateFrom).ToList());
                holidays.AddRange(holidays3Task.Result.Select(h => h.DateFrom).ToList());

                HttpRuntime.Cache.Insert(cacheKey,
                    holidays,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(9999),
                    CacheItemPriority.Normal,
                    null);
            }

            return holidays;
        }

        public static List<DateTime> GetApprovedVacationDays(hrnetModel.User hrproUser)
        {
            var userId = hrproUser.LoggedContact.ID;
            var token = hrproUser.TokenWebApi;

            var vacationDays = new List<DateTime>();

            var vacationRequests = new List<VacationRequest>();
            var vacationRequests1 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year - 1, userId, userId, false, token);
            var vacationRequests2 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year, userId, userId, false, token);
            var vacationRequests3 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year + 1, userId, userId, false, token);
            var temp = new List<VacationRequest>();
            temp.AddRange(vacationRequests1.Result);
            temp.AddRange(vacationRequests2.Result);
            temp.AddRange(vacationRequests3.Result);

            vacationRequests.AddRange(temp.Where(v => v.RequestType == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestType.Vacation &&
                v.RequestStatus == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestStatus.VacationApproved));

            foreach (var req in vacationRequests)
            {
                for (var i = req.DateFrom; i <= req.DateTo; i = i.AddDays(1))
                {
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                        vacationDays.Add(i);
                }
            }

            return vacationDays;
        }

        public static Dictionary<DateTime, string> GetPaidLeaves(hrnetModel.User hrproUser)
        {
            var userId = hrproUser.LoggedContact.ID;
            var token = hrproUser.TokenWebApi;

            var paidLeaves = new Dictionary<DateTime, string>();

            var requests = new List<VacationRequest>();
            var requests1 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year - 1, userId, userId, false, token);
            var requests2 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year, userId, userId, false, token);
            var requests3 = _vacationAPI.GetVacationRequestsAsync(DateTime.Now.Year + 1, userId, userId, false, token);
            var temp = new List<VacationRequest>();
            temp.AddRange(requests1.Result);
            temp.AddRange(requests2.Result);
            temp.AddRange(requests3.Result);

            requests.AddRange(temp.Where(v => v.RequestType == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestType.PaidLeave &&
            v.RequestStatus == hrnetModel.Enums.Vacation.VacationEnums.VacationRequestStatus.PaidLeaveApproved));

            foreach (var req in requests)
            {
                for (var i = req.DateFrom; i <= req.DateTo; i = i.AddDays(1))
                {
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                        paidLeaves.Add(i, req.RequestTypeName ?? "Plaćeni dopust");
                }
            }

            return paidLeaves;
        }
    }
}