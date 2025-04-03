using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class TaxService
    {
        private readonly SQLiteAsyncConnection _database;

        public TaxService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Tax>> GetAllTaxsAsync()
        {
            return await _database.Table<Tax>().ToListAsync();
        }

        public async Task<Tax> GetTaxByIdAsync(int id)
        {
            return await _database.FindAsync<Tax>(id);
        }

        public async Task<int> AddTaxAsync(Tax Tax)
        {
            // Check if TaxName already exists
            var existingTax = await _database.Table<Tax>()
                .FirstOrDefaultAsync(u => u.TaxName == Tax.TaxName);

            if (existingTax != null)
            {
                return 0;
            }

            Tax.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Tax);
        }

        public async Task<bool> UpdateTaxAsync(Tax Tax)
        {
            // Check if TaxName already exists for a different ID
            var existingTax = await _database.Table<Tax>()
                .FirstOrDefaultAsync(u => u.TaxName == Tax.TaxName && u.TaxId != Tax.TaxId);

            if (existingTax != null)
            {
                return false;
            }

            Tax.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Tax);
            return true;
        }

        public async Task<bool> DeleteTaxAsync(int id)
        {
            try
            {
                // Check if the TaxId is used in any related tables
                var isTaxInUse = await _database.Table<SalesDetails>().CountAsync(s => s.TaxId == id) > 0
                                   || await _database.Table<SalesReturnDetails>().CountAsync(sr => sr.TaxId == id) > 0
                                   || await _database.Table<PurchaseDetails>().CountAsync(pm => pm.TaxId == id) > 0
                                   || await _database.Table<PurchaseReturnDetails>().CountAsync(pr => pr.TaxId == id) > 0
                                   || await _database.Table<Product>().CountAsync(p => p.TaxId == id) > 0;

                if (isTaxInUse)
                {
                    // Tax is in use, do not delete
                    return false;
                }

                // Tax is not in use, proceed with deletion
                await _database.DeleteAsync<Tax>(id);
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
