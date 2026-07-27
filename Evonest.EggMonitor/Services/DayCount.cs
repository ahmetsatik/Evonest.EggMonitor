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
        public int ActiveEggId { get; set; }

        private DateTime _startDate = DateTime.MinValue;

        public int elapsedTime
        {
            get
            {
                
                if (_startDate == DateTime.MinValue)
                    return 0;

                return (DateTime.Now.Date - _startDate.Date).Days;
            }
        }

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
                ActiveEggId = active.EggId;
                _startDate = active.StartDate;
            }
        }   

        

    }
}
