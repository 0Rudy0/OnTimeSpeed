using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
	public class HighChartsData
	{
		public string DivName { get; set; }
		public string ChartTitle { get; set; }
		public List<HighChartsSerie> Series { get; set; }
		public List<string> Categories_id { get; set; }
		public List<string> Categories_names { get; set; }
		public List<object> AdditionalCategoriesInfo1 { get; set; }
		public List<object> AdditionalCategoriesInfo2 { get; set; }

		public HighChartsData()
		{
			Series = new List<HighChartsSerie>();
			//var newSerie = new HighChartsSerie();
			//newSerie.ValuesArray = new List<float>();
			//Series.Add(newSerie);
			Categories_id = new List<string>();
			Categories_names = new List<string>();
			AdditionalCategoriesInfo1 = new List<object>();
			AdditionalCategoriesInfo2 = new List<object>();
		}

		public HighChartsData(string divName, string chartTitle)
		{
			this.DivName = divName;
			this.ChartTitle = chartTitle;
			Categories_id = new List<string>();
			Categories_names = new List<string>();
			AdditionalCategoriesInfo1 = new List<object>();
			AdditionalCategoriesInfo2 = new List<object>();
			Series = new List<HighChartsSerie>();
			Series.Add(new HighChartsSerie());
		}

		public class HighChartsSerie
		{
			public string DivName { get; set; }
			public string SerieId { get; set; }
			public string SerieName { get; set; }
			public List<float> ValuesArray { get; set; }
			public string Stack { get; set; }

			public HighChartsSerie()
			{
				ValuesArray = new List<float>();
				Stack = "stack";
			}
		}
	}
}