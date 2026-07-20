using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    [Table("NOTIFICATIONS")]
    internal class NotificationsModel
    {
        [PrimaryKey, Unique]    
        public int Id { get; set; }

        public int EggId { get; set; }

        public  StartingDay { get; set; }

        public
    }
}
