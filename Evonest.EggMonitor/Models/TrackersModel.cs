using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    public class TrackersModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int IncubationId { get; set; }
        public int DayNumber { get; set; }

        public string DateText { get; set; }

        public string TempAndHumidity { get; set; }

        public string Description { get; set; }
    }
}
