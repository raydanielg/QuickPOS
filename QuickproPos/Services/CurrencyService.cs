using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class CurrencyService
    {
        private readonly SQLiteAsyncConnection _database;

        public CurrencyService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Currency>> GetAllCurrencysAsync()
        {
            return await _database.Table<Currency>().ToListAsync();
        }

        public async Task<Currency> GetCurrencyByIdAsync(int id)
        {
            return await _database.FindAsync<Currency>(id);
        }

        public async Task<int> AddCurrencyAsync(Currency Currency)
        {
            // Check if CurrencyName already exists
            var existingCurrency = await _database.Table<Currency>()
                .FirstOrDefaultAsync(u => u.CurrencyName == Currency.CurrencyName);

            if (existingCurrency != null)
            {
                return 0;
            }

            Currency.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Currency);
        }

        public async Task<bool> UpdateCurrencyAsync(Currency Currency)
        {
            // Check if CurrencyName already exists for a different ID
            var existingCurrency = await _database.Table<Currency>()
                .FirstOrDefaultAsync(u => u.CurrencyName == Currency.CurrencyName && u.CurrencyId != Currency.CurrencyId);

            if (existingCurrency != null)
            {
                return false;
            }

            Currency.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Currency);
            return true;
        }

        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            try
            {
                // Check if the CurrencyId is used in the Product table
                var usageCount = await _database.Table<Company>()
                                                 .Where(p => p.CurrencyId == id)
                                                 .CountAsync();

                if (usageCount > 0)
                {
                    // Currency is in use, do not delete
                    return false; // Return false to indicate deletion was not performed
                }

                // Currency is not in use, proceed with deletion
                await _database.DeleteAsync<Currency>(id);
                return true; // Return true to indicate successful deletion
            }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error deleting Currency: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
}


    }
}
