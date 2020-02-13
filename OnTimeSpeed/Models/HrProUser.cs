using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public class HrProUser
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
        public string TokenWebApi { get; set; }

        public Contact LoggedContact { get; set; }
        public string Culture { get; set; }

        public string ErrorMessage { get; set; }
        public bool ForceLogOff { get; set; }
    }

    public class Contact
    {
        public string ProfilePicture { get; set; }
        public string ProfilePictureFallback { get; set; }
    }
}