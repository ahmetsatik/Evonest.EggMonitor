using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SQLite;

namespace Evonest.EggMonitor.Models
{
    [SQLite.Table("Incubations")] // Veri tabanındaki tablo adı
    public class IncubationPresetModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Her kuluçkanın benzersiz kimliği

        [SQLite.MaxLength(100)]
        public string Name { get; set; } // Kuluçka Adı (Örn: "Sarı Kızın Yumurtaları")

        [NotNull]
        public string Species { get; set; } // Kuş Cinsi (Örn: "Tavuk", "Ördek")

        [NotNull]
        public int EggCount { get; set; } // Yumurta Sayısı

        [NotNull]
        public DateTime StartDate { get; set; } // Başlangıç Tarihi ve Saeti

        public bool IsAutomaticTurner { get; set; } // Otomatik çevirme var mı? (True/False)

        public bool IsActive { get; set; } // Kuluçka hala devam ediyor mu, bitti mi?

        // --- İlerleyen Aşamalar İçin Telemetri ve Analiz Sütunları ---
        public int? HatchedEggCount { get; set; } // Çıkan (Yumurtadan ayrılan) civciv sayısı (Kuluçka bitiminde doldurulacak)

        public string Notes { get; set; } // Kullanıcının süreçle ilgili alacağı ekstra notlar
    }
}
