using Evonest.EggMonitor.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;

namespace Evonest.EggMonitor.Services
{
    public class TrackersService
    {


        private SQLiteAsyncConnection _database;
        private SQLiteAsyncConnection _eggDb;



        public async Task<int> SaveTrackerAsync(TrackersModel track)
        {
            await InitAsync();
            int currentDay = track.DayNumber;
            int targetIncubationId = track.IncubationId;

            var existingRecord = await _database.Table<TrackersModel>()
                                        .Where(t => t.IncubationId == targetIncubationId)
                                        .Where(t => t.DayNumber == currentDay)
                                        .FirstOrDefaultAsync();
            if (existingRecord != null)
            {
                track.Id = existingRecord.Id;
                return await _database.UpdateAsync(track);
            }

            else
            {

                return await _database.InsertAsync(track);
            }
        }


        public async Task<List<TrackersModel>> GetTrackersAsync(int incubationId)
        {
            await InitAsync();

            return await _database.Table<TrackersModel>()
                                  .Where(t => t.IncubationId == incubationId)
                                  .OrderBy(t => t.DayNumber)
                                  .ToListAsync();
        }



        private async Task InitAsync()
        {

            if (_database != null)
                return;


            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
            _database = new SQLiteAsyncConnection(dbPath);


            await _database.CreateTableAsync<TrackersModel>();
        }

        public async Task<bool> CheckRecordExistsAsync(int incubationId, int dayNumber)
        {

            await InitAsync();

            var existingRecord = await _database.Table<TrackersModel>()
                                                .Where(t => t.IncubationId == incubationId &&
                                                            t.DayNumber == dayNumber)
                                                .FirstOrDefaultAsync();

            return existingRecord != null;
        }
    

        public async Task<String> TempAndHumunityCheckAsync(int eggId, int dayNumber, string currentHumidity, string temp)
        {
            double.TryParse(temp, out double tempValue);
            double.TryParse(currentHumidity, out double humValue);

            string warningMessage = "";


            string dbPath2 = Path.Combine(FileSystem.AppDataDirectory, "EggInfo.db");
            _eggDb = new SQLiteAsyncConnection(dbPath2);

            var currentStage = await _eggDb.Table<InstructionModel>()
                                .Where(c => c.EggID == eggId
                                         && c.StartingDay <= dayNumber
                                         && c.EndingDay >= dayNumber)
                                .FirstOrDefaultAsync();

            if (currentStage == null || currentStage.MaxTemp == 0)
                return "";

            if (tempValue > currentStage.MaxTemp)
                warningMessage += $"🌡️ Sıcaklık yüksek! Maksimum: {currentStage.MaxTemp}°C olmalı.\n";
            else if (tempValue < currentStage.MinTemp)
                warningMessage += $"❄️ Sıcaklık düşük! Minimum: {currentStage.MinTemp}°C olmalı.\n";

            if (humValue < currentStage.MinHumidity)
                warningMessage += $"🏜️ Nem düşük! Minimum: %{currentStage.MinHumidity} olmalı.\n";
            else if (humValue > currentStage.MaxHumidity)
                warningMessage += $"💧 Nem yüksek! Maksimum: %{currentStage.MaxHumidity} olmalı.\n";


            return warningMessage;

        }

    }
}
