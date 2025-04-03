using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class BrandService
    {
        private readonly SQLiteAsyncConnection _database;

        public BrandService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _database.Table<Brand>().ToListAsync();
        }

        public async Task<Brand> GetBrandByIdAsync(int id)
        {
            return await _database.FindAsync<Brand>(id);
        }

        public async Task<int> AddBrandAsync(Brand Brand)
        {
            // Check if BrandName already exists
            var existingBrand = await _database.Table<Brand>()
                .FirstOrDefaultAsync(u => u.Name == Brand.Name);

            if (existingBrand != null)
            {
                return 0;
            }

            Brand.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Brand);
        }

        public async Task<bool> UpdateBrandAsync(Brand Brand)
        {
            // Check if BrandName already exists for a different ID
            var existingBrand = await _database.Table<Brand>()
                .FirstOrDefaultAsync(u => u.Name == Brand.Name && u.BrandId != Brand.BrandId);

            if (existingBrand != null)
            {
                return false;
            }

            Brand.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Brand);
            return true;
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            try
            {
                // Check if the BrandId is used in the Product table
                var usageCount = await _database.Table<Product>()
                                                 .Where(p => p.BrandId == id)
                                                 .CountAsync();

                if (usageCount > 0)
                {
                    // Brand is in use, do not delete
                    return false; // Return false to indicate deletion was not performed
                }

                // Brand is not in use, proceed with deletion
                await _database.DeleteAsync<Brand>(id);
                return true; // Return true to indicate successful deletion
            }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error deleting Brand: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
}


    }
}
