using Evonest.EggMonitor.Models;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Evonest.EggMonitor.Services
{

    internal class NotificationsService
    {
        private readonly SQLiteAsyncConnection _db;

        public NotificationsService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "EggInfo.db");
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public async Task ScheduleNotificationsAsync(int eggId, int hatchId, DateTime startDate)
        {


            bool isPermissionGranted = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (!isPermissionGranted)
            {
                isPermissionGranted = await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            if (!isPermissionGranted) return;

            var timeTestRequest = new NotificationRequest
            {
                NotificationId = 998,
                Title = "Zamanlayıcı Testi ⏳",
                Description = "Bunu butona bastıktan tam 10 saniye sonra görmelisin.",
                Schedule = new NotificationRequestSchedule
                {
                    // Şimdiki zamana tam 10 saniye ekliyoruz
                    NotifyTime = DateTime.Now.AddSeconds(10)
                }
            };
            await LocalNotificationCenter.Current.Show(timeTestRequest);

            try
            {

                var sablonlar = await _db.Table<NotificationsModel>()
                                         .Where(n => n.EggID == eggId)
                                         .ToListAsync();


                foreach (var sablon in sablonlar)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = (hatchId * 1000) + sablon.ID,
                        Title = sablon.Title,
                        Description = sablon.Message,
                        Schedule = new NotificationRequestSchedule
                        {
                            //NotifyTime = startDate.Date.AddDays(sablon.DayNumber).AddHours(sablon.TriggerHour)
                            NotifyTime = DateTime.Now.AddMinutes(sablon.DayNumber)
                        }
                    };

                    await LocalNotificationCenter.Current.Show(request);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bildirim kurma hatası: {ex.Message}");
            }
        }
    }
}
