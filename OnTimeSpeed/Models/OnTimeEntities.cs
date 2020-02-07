using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{    public class WorkLog
    {
        public Item item { get; set; }
        public Project project { get; set; }
        public Release release { get; set; }
        public User user { get; set; }
        public Work_Log_Type work_log_type { get; set; }
        public Work_Done work_done { get; set; }
        public int id { get; set; }
        public string description { get; set; }
        public DateTime date_time { get; set; }

        public override string ToString()
        {
            return $"{date_time.Date.ToShortDateString()} - {item.name}: {work_done.duration_minutes / 60} h";
        }
    }

    public class Item
    {
        public int id { get; set; }
        public string item_type { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public Estimated_Duration estimated_duration { get; set; }
        public Remaining_Duration remaining_duration { get; set; }
        public Customer customer { get; set; }
        public Reported_By_Customer_Contact reported_by_customer_contact { get; set; }
    }

    public class Estimated_Duration
    {
        public float duration_minutes { get; set; }
    }

    public class Remaining_Duration
    {
        public float duration_minutes { get; set; }
    }

    public class Customer
    {
        public object company_name { get; set; }
        public object id { get; set; }
    }

    public class Reported_By_Customer_Contact
    {
        public string name { get; set; }
        public object id { get; set; }
        public object email { get; set; }
        public object phone { get; set; }
    }

    public class Project
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public object name { get; set; }
        public object path { get; set; }
    }

    public class User
    {
        public string Token { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string ImageUrl
        {
            get
            {
                return $"{ConfigurationManager.AppSettings["ontimeApiUrl"]}/users/{id}/image?size=24";
            }
        }
    }

    public class Work_Log_Type
    {
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Work_Done
    {
        public string text { get; set; }
        public float duration_minutes { get; set; }
        public Time_Unit time_unit { get; set; }
        public float duration { get; set; }
    }

    public class Time_Unit
    {
        public string abbreviation { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

}