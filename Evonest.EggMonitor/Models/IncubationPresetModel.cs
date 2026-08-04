using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Transactions;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    [SQLite.Table("Incubations")]
    public class IncubationPresetModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100)]
        public string Name { get; set; }

        [NotNull]
        public string Species { get; set; }

        [NotNull]
        public int EggId { get; set; }

        [NotNull]
        public int EggCount { get; set; } 

        [NotNull]
        public DateTime StartDate { get; set; } 

        public bool IsAutomaticTurner { get; set; } 

        public bool IsActive { get; set; } 

        public int? HatchedEggCount { get; set; } 

        public string Notes { get; set; }
        
        public int FertileEggCount { get; set; }

        public int HatchedChickCount { get; set; }

        public int IsFinished { get; set; }

        public double SuccessRate { get; set; }
    }
}
