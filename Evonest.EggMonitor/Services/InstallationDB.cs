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
                                      .Where(x => x.IsActive == true)
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

        public async Task<List<IncubationPresetModel>> GetAllHatchesAsync()
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
                _database = new SQLiteAsyncConnection(dbPath);
            }

            return await _database.Table<IncubationPresetModel>()
                                  .Where(x => x.IsFinished != true)
                                  .ToListAsync();
        }

        public async Task SwitchActiveHatchAsync(int newActiveId)
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Installation.db3");
                _database = new SQLiteAsyncConnection(dbPath);
            }

            var allHatches = await _database.Table<IncubationPresetModel>().ToListAsync();
            foreach (var hatch in allHatches)
            {
                if (hatch.IsActive)
                {
                    hatch.IsActive = false;
                    await _database.UpdateAsync(hatch);
                }
            }

            var selectedHatch = allHatches.FirstOrDefault(h => h.Id == newActiveId);
            if (selectedHatch != null)
            {
                selectedHatch.IsActive = true;
                await _database.UpdateAsync(selectedHatch);
            }
        }

        public async Task<List<InstructionModel>> GetInstructionsByEggIdAsync(int eggId)
        {
            await InitAsync();

            try
            {
                return await _database.Table<InstructionModel>()
                                      .Where(x => x.EggID == eggId)
                                      .ToListAsync();
            }
            catch (Exception)
            {
                return new List<InstructionModel>();
            }
        }

        public async Task<List<IncubationPresetModel>> GetFinishedHatchesAsync()
        {
            await InitAsync();

            try
            {
                return await _database.Table<IncubationPresetModel>()
                                      .Where(x => x.IsFinished == true)
                                      .OrderByDescending(x => x.StartDate)
                                      .ToListAsync();
            }
            catch (Exception)
            {
                return new List<IncubationPresetModel>();
            }
        }
        public async Task<int> UpdateHatchAsync(IncubationPresetModel hatch)
        {
            await InitAsync();

            try
            {
                return await _database.UpdateAsync(hatch);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

}