using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Services
{
    public class CategoryService
    {
        private readonly SQLiteAsyncConnection _database;

        public CategoryService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<ProductGroupView>> GetAllProductGroupsAsync()
        {
            // Fetch all product groups
            var productGroups = await _database.Table<ProductGroup>().ToListAsync();

            // Perform a left join to include top-level groups
            var result = productGroups
                .GroupJoin(
                    productGroups, // Join with the same list
                    pg => pg.GroupUnder, // Outer key (GroupUnder)
                    pgUnder => pgUnder.GroupId, // Inner key (GroupId of parent)
                    (pg, matchingParents) => new ProductGroupView
                    {
                        GroupId = pg.GroupId,
                        GroupName = pg.GroupName,
                        GroupUnder = pg.GroupUnder,
                        Under = matchingParents.FirstOrDefault()?.GroupName ?? "No Parent", // Map parent GroupName or default to "No Parent"
                        Image = pg.Image,
                        Narration = pg.Narration,
                        AddedDate = pg.AddedDate,
                        ModifyDate = pg.ModifyDate
                    }
                )
                .ToList();

            return result;
        }

        public async Task<ProductGroup> GetProductGroupByIdAsync(int id)
        {
            return await _database.FindAsync<ProductGroup>(id);
        }

        public async Task<int> AddProductGroupAsync(ProductGroup ProductGroup)
        {
            // Check if ProductGroupName already exists
            var existingProductGroup = await _database.Table<ProductGroup>()
                .FirstOrDefaultAsync(u => u.GroupName == ProductGroup.GroupName);

            if (existingProductGroup != null)
            {
                return 0;
            }

            ProductGroup.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(ProductGroup);
        }

        public async Task<bool> UpdateProductGroupAsync(ProductGroup ProductGroup)
        {
            // Check if ProductGroupName already exists for a different ID
            var existingProductGroup = await _database.Table<ProductGroup>()
                .FirstOrDefaultAsync(u => u.GroupName == ProductGroup.GroupName && u.GroupId != ProductGroup.GroupId);

            if (existingProductGroup != null)
            {
                return false;
            }

            ProductGroup.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(ProductGroup);
            return true;
        }

        public async Task<bool> DeleteProductGroupAsync(int id)
        {
            try
            {
                // Check if the ProductGroupId is used in the Product table
                var usageCount = await _database.Table<Product>()
                                                 .Where(p => p.GroupId == id)
                                                 .CountAsync();

                if (usageCount > 0)
                {
                    // ProductGroup is in use, do not delete
                    return false; // Return false to indicate deletion was not performed
                }

                // ProductGroup is not in use, proceed with deletion
                await _database.DeleteAsync<ProductGroup>(id);
                return true; // Return true to indicate successful deletion
            }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error deleting ProductGroup: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
}


    }
}
