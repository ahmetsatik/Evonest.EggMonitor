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



        public async Task<int> SaveTrackerAsync(TrackersModel track)
        {
            InitAsync();
            var dayCountService = new DayCount();
            int currentDay = dayCountService.elapsedTime;
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
    }


}
