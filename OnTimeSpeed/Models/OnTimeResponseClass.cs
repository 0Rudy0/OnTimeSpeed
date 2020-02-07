using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public class OnTimeResponseClass
    {
        public object data { get; set; }
        public MetaData metadata { get; set; }

        public string ErrorCode { get; set; }
    }

    public class MetaData
    {
        public int total_count { get; set; }
        public int page { get; set; }
        public int page_size { get; set; }
        public float minutes_worked { get; set; }
        public float minutes_estimated { get; set; }
        public float minutes_remaining { get; set; }
        public float percent_complete { get; set; }
    }

}