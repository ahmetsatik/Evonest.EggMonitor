using System;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    [Table("NOTIFICATIONS")]
    public class NotificationsModel
    {
        [PrimaryKey, Unique]
        [Column("ID")]
        public int ID { get; set; }

        [Column("EggID")]
        public int EggID { get; set; }

        [Column("DayNumber")]
        public int DayNumber { get; set; }

        [Column("TriggerHour")]
        public int TriggerHour { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Message")]
        public string Message { get; set; }
    }
}