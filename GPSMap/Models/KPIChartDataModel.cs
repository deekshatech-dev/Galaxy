﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPSMap.Models
{
    public class KPIChartDataModel
    {
        public ChartSearchModel SearchModel { get; set; }

        public ChartValues ChartValues { get; set; }

        public SelectList KPIValues { get; set; }
        public Guid UniqueId { get; set; }
        
    }
    public class ChartKPIValues
    {
        public string KPI { get; set; }
        public ChartValues ChartData { get; set; }

    }

    public class ChartValues
    {
        public List<string> Labels { get; set; }
        public List<string> ChartData { get; set; }

    }

    public class ChartRequest
    {
        public string Trend { get; set; }
        public int[] KpiId{ get; set; }
        public string Date { get; set; }
        public bool ShowSeparate { get; set; }
    }
}