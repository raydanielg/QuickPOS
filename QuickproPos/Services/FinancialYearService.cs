using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class FinancialYearService
    {
        private readonly SQLiteAsyncConnection _database;

        public FinancialYearService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<FinancialYear>> GetAllFinancialYearsAsync()
        {
            return await _database.Table<FinancialYear>().ToListAsync();
        }

        public async Task<FinancialYear> GetFinancialYearByIdAsync(int id)
        {
            return await _database.FindAsync<FinancialYear>(id);
        }

        public async Task<int> AddFinancialYearAsync(FinancialYear financialYear)
        {
            // Check if FiscalYear already exists
            var existingFinancialYear = await _database.Table<FinancialYear>()
                .FirstOrDefaultAsync(u => u.FiscalYear == financialYear.FiscalYear);

            if (existingFinancialYear != null)
            {
                // If the fiscal year already exists, return 0
                return 0;
            }

            // Set the AddedDate for the new financial year
            financialYear.AddedDate = DateTime.UtcNow;

            // Insert the new financial year into the database
            int intSave = await _database.InsertAsync(financialYear);

            if (intSave > 0)
            {
                // Retrieve the first company entry from the database
                var company = await _database.Table<Company>().FirstOrDefaultAsync();

                if (company != null)
                {
                    // Update the company's FinancialYearId, StartDate, and ValidDate
                    company.FinancialYearId = financialYear.FinancialYearId;
                    company.StartDate = financialYear.FromDate;
                    company.ValidDate = financialYear.ToDate;

                    // Update the company record
                    int updateResult = await _database.UpdateAsync(company);

                    return updateResult;
                }
            }
            return intSave;

            // If we reach here, it means something went wrong with the insert or update
        }


        public async Task<bool> UpdateFinancialYearAsync(FinancialYear FinancialYear)
        {
            // Check if FinancialYearName already exists for a different ID
            var existingFinancialYear = await _database.Table<FinancialYear>()
                .FirstOrDefaultAsync(u => u.FiscalYear == FinancialYear.FiscalYear && u.FinancialYearId != FinancialYear.FinancialYearId);

            if (existingFinancialYear != null)
            {
                return false;
            }
            else
            {

                FinancialYear.ModifyDate = DateTime.UtcNow;
                await _database.UpdateAsync(FinancialYear);
                // Retrieve the first company entry from the database
                var company = await _database.Table<Company>().FirstOrDefaultAsync();

                if (company != null)
                {
                    // Update the company's FinancialYearId, StartDate, and ValidDate
                    company.FinancialYearId = FinancialYear.FinancialYearId;
                    company.StartDate = FinancialYear.FromDate;
                    company.ValidDate = FinancialYear.ToDate;

                    // Update the company record
                    int updateResult = await _database.UpdateAsync(company);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteFinancialYearAsync(int id)
        {
            try
            {
                // Check if the FinancialYearId is used in the Product table
                var usageCount = await _database.Table<Company>()
                                                 .Where(p => p.FinancialYearId == id)
                                                 .CountAsync();

                if (usageCount > 0)
                {
                    // FinancialYear is in use, do not delete
                    return false; // Return false to indicate deletion was not performed
                }

                // FinancialYear is not in use, proceed with deletion
                await _database.DeleteAsync<FinancialYear>(id);
                return true; // Return true to indicate successful deletion
            }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error deleting FinancialYear: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
}


    }
}
