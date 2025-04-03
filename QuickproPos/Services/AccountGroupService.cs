using QuickproPos.Data.Account;
using QuickproPos.Data.AccountView;
using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class AccountGroupService
    {
        private readonly SQLiteAsyncConnection _database;

        public AccountGroupService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<AccountGroupView>> GetAllAccountGroupAsync()
        {
            // Fetch all product groups
            var accountGroups = await _database.Table<AccountGroup>().ToListAsync();

            // Perform a left join to include top-level groups
            var result = accountGroups
                .GroupJoin(
                    accountGroups, // Join with the same list
                    pg => pg.GroupUnder, // Outer key (GroupUnder)
                    pgUnder => pgUnder.AccountGroupId, // Inner key (GroupId of parent)
                    (pg, matchingParents) => new AccountGroupView
                    {
                        AccountGroupId = pg.AccountGroupId,
                        AccountGroupName = pg.AccountGroupName,
                        GroupUnder = pg.GroupUnder,
                        Under = matchingParents.FirstOrDefault()?.AccountGroupName ?? "Primary", // Map parent GroupName or default to "No Parent"
                        Nature = pg.Nature
                    }
                )
                .ToList();

            return result;
        }

        public async Task<AccountGroup> GetAccountGroupByIdAsync(int id)
        {
            return await _database.FindAsync<AccountGroup>(id);
        }

        public async Task<int> AddAccountGroupAsync(ProductGroup ProductGroup)
        {
            // Check if AccountGroupName already exists
            var existingProductGroup = await _database.Table<ProductGroup>()
                .FirstOrDefaultAsync(u => u.GroupName == ProductGroup.GroupName);

            if (existingProductGroup != null)
            {
                return 0;
            }

            ProductGroup.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(ProductGroup);
        }

        public async Task<bool> UpdateAccountGroupAsync(AccountGroup AccountGroup)
        {
            // Check if AccountGroupName already exists for a different ID
            var existingAccountGroup = await _database.Table<AccountGroup>()
                .FirstOrDefaultAsync(u => u.AccountGroupName == AccountGroup.AccountGroupName && u.AccountGroupId != AccountGroup.AccountGroupId);

            if (existingAccountGroup != null)
            {
                return false;
            }

            AccountGroup.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(AccountGroup);
            return true;
        }
    }
}
