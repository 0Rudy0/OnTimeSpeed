using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public class Error
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }

        public Error() { }

        public override string ToString()
        {
            return $"{ExceptionMessage} {StackTrace}";
        }
    }
}