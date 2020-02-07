using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace OnTimeSpeed.Utils
{
    public static class AppSettings
    {
		public static string Get(string key)
		{
			try
			{
				return WebConfigurationManager.AppSettings[key].ToString();
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static bool GetBool(string key)
		{
			try
			{
				return WebConfigurationManager.AppSettings[key] != null && Boolean.Parse(WebConfigurationManager.AppSettings[key]);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static int GetInt(string key)
		{
			try
			{
				return Int32.Parse(WebConfigurationManager.AppSettings[key]);
			}
			catch (Exception)
			{
				return 0;
			}
		}
	}
}