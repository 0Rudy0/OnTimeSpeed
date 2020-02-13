using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Code
{
    public class SearchString
    {
        public string Name { get; set; }
        public IEnumerable<string> SearchStrings { get; set; }
    }

    public static class SearchStrings
    {
        private static List<SearchString> searchStrings;

        public static List<SearchString> Get()
        {
            if (searchStrings == null)
            {
                string json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory  + "/config/searchStrings.json");
                searchStrings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchString>>(json);
            }

            return searchStrings;
        }
    }
}