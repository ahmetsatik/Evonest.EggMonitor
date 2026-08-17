using Evonest.EggMonitor.Models;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;

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

                return (DateTime.Now.Date - _startDate.Date).Days + 1;
            }
        }

        public async Task DayCounter()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            var active = await _database.Table<IncubationPresetModel>()
                                               .Where(x => x.IsActive == true)
                                               .FirstOrDefaultAsync();

            if (active != null)
            {
                ActiveEggId = active.EggId;
                _startDate = active.StartDate;
            }
            else
            {
                ActiveEggId = 0;
                _startDate = DateTime.MinValue;
            }
        }
    }
}