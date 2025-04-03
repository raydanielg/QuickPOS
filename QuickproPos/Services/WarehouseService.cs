using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class WarehouseService
    {
        private readonly SQLiteAsyncConnection _database;

        public WarehouseService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync()
        {
            return await _database.Table<Warehouse>().ToListAsync();
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            return await _database.FindAsync<Warehouse>(id);
        }

        public async Task<int> AddWarehouseAsync(Warehouse Warehouse)
        {
            // Check if WarehouseName already exists
            var existingWarehouse = await _database.Table<Warehouse>()
                .FirstOrDefaultAsync(u => u.Name == Warehouse.Name);

            if (existingWarehouse != null)
            {
                return 0;
            }

            Warehouse.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Warehouse);
        }

        public async Task<bool> UpdateWarehouseAsync(Warehouse Warehouse)
        {
            // Check if WarehouseName already exists for a different ID
            var existingWarehouse = await _database.Table<Warehouse>()
                .FirstOrDefaultAsync(u => u.Name == Warehouse.Name && u.WarehouseId != Warehouse.WarehouseId);

            if (existingWarehouse != null)
            {
                return false;
            }

            Warehouse.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Warehouse);
            return true;
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            try
            {
                // Check if the WarehouseId is used in any related tables
                var isWarehouseInUse = await _database.Table<SalesMaster>().CountAsync(s => s.WarehouseId == id) > 0
                                   || await _database.Table<SalesReturnMaster>().CountAsync(sr => sr.WarehouseId == id) > 0
                                   || await _database.Table<PurchaseMaster>().CountAsync(pm => pm.WarehouseId == id) > 0
                                   || await _database.Table<PurchaseReturnMaster>().CountAsync(pr => pr.WarehouseId == id) > 0
                                   || await _database.Table<Product>().CountAsync(p => p.WarehouseId == id) > 0;

                if (isWarehouseInUse)
                {
                    // Warehouse is in use, do not delete
                    return false;
                }

                // Warehouse is not in use, proceed with deletion
                await _database.DeleteAsync<Warehouse>(id);
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
