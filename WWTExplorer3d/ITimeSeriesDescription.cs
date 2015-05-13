using System;
namespace TerraViewer
{
    interface ITimeSeriesDescription
    {
        bool IsTimeSeries { get; set; }
        DateTime SeriesStartTime { get;  }
        DateTime SeriesEndTime { get; }
       
        TimeSpan TimeStep { get;  }
    }
}
