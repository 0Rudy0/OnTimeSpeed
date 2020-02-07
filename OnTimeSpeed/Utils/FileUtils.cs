
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace OnTimeSpeed.Utils
{
	static class FileUtils
	{
		private static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

		/// <summary>
		/// Writes to log file asynchronously and with file locks
		/// </summary>
		/// <param name="s">String to write to log file</param>
		public static void WriteToLog(string s, string fileName)
		{
			// create new thread that appends string to a file
			new Thread(str =>
			{
				string path = HttpRuntime.AppDomainAppPath + "log\\" + fileName;

				try
				{
					// Try to get exclusive write lock to the file.
					// Keep trying for 30 milliseconds.
					rwl.TryEnterWriteLock(30);
					try
					{
						if (!File.Exists(path))
						{
							using (StreamWriter sw = File.AppendText(path))
							{
								var external = "<script type=\"text/javascript\" src=\"files/jquery-2.2.4.min.js\"></script>\n";
								external += "<script type=\"text/javascript\" src=\"files/Kendo/js/kendo.custom-2015.3.930.js\"></script>\n";
								external += "<script type=\"text/javascript\" src=\"../Scripts/bootstrap.min.js\"></script>\n";
								external += "<script type=\"text/javascript\" src=\"files/moment.js\"></script>\n";
								external += "<script type=\"text/javascript\" src=\"files/log.js\"></script>\n";
								external += "<link type=\"text/css\" rel=\"stylesheet\" href=\"files/log.css\"/>\n";
								external += "<link type=\"text/css\" rel=\"stylesheet\" href=\"../Content/Bootstrap/bootstrap.min.css\"/>\n";
								external += "<link type=\"text/css\" rel=\"stylesheet\" href=\"files/Kendo/css/kendo.common.css\"/>\n";
								external += "<link type=\"text/css\" rel=\"stylesheet\" href=\"files/Kendo/css/kendo.silver.css\"/>\n";
								external += "<head><title>" + fileName.Replace(".html", "") + "</title></head>";
								sw.WriteLine("<!DOCTYPE html>\n<!-- saved from url=(0014)about:internet -->"); //IE compatibility comment
								sw.WriteLine(external);
								sw.WriteLine(str);
							}
						}
						else
						{
							using (StreamWriter sw = File.AppendText(path))
							{
								sw.WriteLine(str);
							}
						}

					}
					catch (Exception)
					{
						//Nema smisla pisat u log da se ne može pisati u log - sonarQube me je natjerao da ovo napišem :)
					}
				}
				finally
				{
					if (rwl.IsWriteLockHeld)
					{
						// release write lock
						rwl.ExitWriteLock();
					}
				}

			}).Start(s);
		}
	}
}