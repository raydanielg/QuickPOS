using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class InvoiceSettingService
    {
        private readonly SQLiteAsyncConnection _database;

        public InvoiceSettingService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<InvoiceSetting>> GetAllInvoiceSettingsAsync()
        {
            return await _database.Table<InvoiceSetting>().ToListAsync();
        }
        public async Task<InvoiceSetting> GetInvoiceSettingByIdAsync(int id)
        {
            return await _database.FindAsync<InvoiceSetting>(id);
        }

        public async Task<bool> UpdateInvoiceSettingAsync(InvoiceSetting InvoiceSetting)
        {
            // Check if InvoiceSettingName already exists for a different ID
            var existingInvoiceSetting = await _database.Table<InvoiceSetting>()
                .FirstOrDefaultAsync(u => u.VoucherTypeName == InvoiceSetting.VoucherTypeName && u.VoucherTypeId != InvoiceSetting.VoucherTypeId);

            if (existingInvoiceSetting != null)
            {
                return false;
            }

            await _database.UpdateAsync(InvoiceSetting);
            return true;
        }
    }
}
