using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
	public class JsLogEvent
	{
		public string Title { get; set; }
		public long Date { get; set; }
		public DateTime DateActual { get; set; }
		public string Element { get; set; }
		public string Url { get; set; }
		public string Page { get; set; }
		public string ResponseStatus { get; set; }
		public string ResponseDuration { get; set; }
		public string ErrorMessage { get; set; }
		public string Stack { get; set; }
	}

	public class JsLogEventCollection : List<JsLogEvent> { }
}