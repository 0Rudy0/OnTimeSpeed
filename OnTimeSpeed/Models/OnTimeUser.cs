using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public class OnTimeUser
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string login_id { get; set; }
        public string windows_id { get; set; }
        public bool use_windows_auth { get; set; }
        public bool built_in_account { get; set; }
        public bool is_active { get; set; }
        public bool is_locked { get; set; }
        public DateTime last_login_date_time { get; set; }
        public int failed_logins { get; set; }
        public DateTime created_date_time { get; set; }
        public DateTime last_updated_date_time { get; set; }
        public Security_Roles[] security_roles { get; set; }
    }

    public class Security_Roles
    {
        public string name { get; set; }
        public int id { get; set; }
    }

}