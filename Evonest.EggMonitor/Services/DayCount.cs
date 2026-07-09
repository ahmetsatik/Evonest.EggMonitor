using Evonest.EggMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evonest.EggMonitor.Services
{
    internal class DayCount
    {
        private SQLiteAsyncConnection _database;

        public int elapsedTime { get; set; }

        public int ActiveEggId { get; set; }

        public async Task DayCounter()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            var active = await _database.Table<IncubationPresetModel>()
                                               .Where(x => x.IsActive == true)
                                               .FirstOrDefaultAsync();

            ActiveEggId= active.EggId;


            if (active !=null)
            {
                elapsedTime = (DateTime.Now - active.StartDate).Days;
                
            }

            await Task.Delay(TimeSpan.FromMinutes(1));

        }

        

    }
}
