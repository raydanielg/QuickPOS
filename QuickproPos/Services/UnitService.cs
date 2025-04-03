using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class UnitService
    {
        private readonly SQLiteAsyncConnection _database;

        public UnitService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _database.Table<Unit>().ToListAsync();
        }

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            return await _database.FindAsync<Unit>(id);
        }

        public async Task<int> AddUnitAsync(Unit Unit)
        {
            // Check if UnitName already exists
            var existingUnit = await _database.Table<Unit>()
                .FirstOrDefaultAsync(u => u.UnitName == Unit.UnitName);

            if (existingUnit != null)
            {
                return 0;
            }

            Unit.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Unit);
        }

        public async Task<bool> UpdateUnitAsync(Unit Unit)
        {
            // Check if UnitName already exists for a different ID
            var existingUnit = await _database.Table<Unit>()
                .FirstOrDefaultAsync(u => u.UnitName == Unit.UnitName && u.UnitId != Unit.UnitId);

            if (existingUnit != null)
            {
                return false;
            }

            Unit.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Unit);
            return true;
        }

        public async Task<bool> DeleteUnitAsync(int id)
        {
            try
            {
                // Check if the UnitId is used in any related tables
                var isUnitInUse = await _database.Table<SalesDetails>().CountAsync(s => s.UnitId == id) > 0
                                   || await _database.Table<SalesReturnDetails>().CountAsync(sr => sr.UnitId == id) > 0
                                   || await _database.Table<PurchaseDetails>().CountAsync(pm => pm.UnitId == id) > 0
                                   || await _database.Table<PurchaseReturnDetails>().CountAsync(pr => pr.UnitId == id) > 0
                                   || await _database.Table<Product>().CountAsync(p => p.UnitId == id) > 0;

                if (isUnitInUse)
                {
                    // Unit is in use, do not delete
                    return false;
                }

                // Unit is not in use, proceed with deletion
                await _database.DeleteAsync<Unit>(id);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error deleting Product: {ex.Message}");
                throw;
            }
        }


    }
}
