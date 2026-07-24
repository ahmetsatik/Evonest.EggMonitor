using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Evonest.EggMonitor.Models;


namespace Evonest.EggMonitor.Services
{
    public class InstallationDB
    {
        private SQLiteAsyncConnection _database;

        public InstallationDB()
        {
        }

        private async Task InitAsync()
        {
            if (_database != null) { return; }

                else
                {
                    string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
                    _database = new SQLiteAsyncConnection(dbPath);

                    await _database.CreateTableAsync<IncubationPresetModel>();
                    await _database.CreateTableAsync<TrackersModel>();
                }

        }

        public async Task<int> SaveHatchAsync(IncubationPresetModel hatch)
        {
            await InitAsync();
            return await _database.InsertAsync(hatch);
        }


        public async Task<IncubationPresetModel> GetActiveHatchAsync()
        {
            await InitAsync();

            try
            {
                return await _database.Table<IncubationPresetModel>()
                                      .OrderByDescending(x => x.Id)
                                      .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<IncubationPresetModel> ShowHatch()
        {
            await InitAsync();
            try
            {
                return await _database.Table<IncubationPresetModel>()
                                      .OrderByDescending(x => x.Id)
                                      .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task DeactivateCurrentHatchAsync()
        {
            await InitAsync();
            var activeHatch = await _database.Table<IncubationPresetModel>()
                                             .Where(t => t.IsActive == true)
                                             .FirstOrDefaultAsync();

            if (activeHatch != null)
            {
                activeHatch.IsActive = false;
                await _database.UpdateAsync(activeHatch);
            }
        }
    }
}