using System;
using System.Collections.Generic;
using System.Text;

namespace Evonest.EggMonitor.Models
{
    public class TrackersModel
    {
        public int DayNumber { get; set; }

        public string DateText { get; set; }

        public string TempAndHumidity { get; set; }

        public string Information { get; set; }
    }
}
