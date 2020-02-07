using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public class TokenData
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public UserData data { get; set; }
    }

    public class UserData
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }

}