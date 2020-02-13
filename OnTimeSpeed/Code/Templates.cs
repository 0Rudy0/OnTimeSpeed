using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Code
{
    public class Template
    {
        public string Name { get; set; }
        public IEnumerable<string> Templates { get; set; }
    }

    public static class Templates
    {
        private static List<Template> templates;

        public static List<Template> Get()
        {
            if (templates == null)
            {
                string json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory  + "/config/templates.json");
                templates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Template>>(json);
            }

            return templates;
        }
    }
}