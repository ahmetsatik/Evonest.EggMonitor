using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    [Table("INSTRUCTIONS")]
    public class InstructionModel
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int ID { get; set; }

        [Column("EggID")]
        public int EggID { get; set; }

        [Column("StartingDay")]
        public int StartingDay { get; set; }

        [Column("EndingDay")]
        public int EndingDay { get; set; }

        [Column("Instructions")]
        public string Instructions { get; set; }

        [Column("MinTemp")]
        public double MinTemp { get; set; }

        [Column("MaxTemp")]
        public double MaxTemp { get; set; }

        [Column("MinHumidity")]
        public double MinHumidity { get; set; }

        [Column("MaxHumidty")]
        public double MaxHumidity { get; set; }
    }
}
