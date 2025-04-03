using QuickproPos.Data.User;
using SQLite;

namespace QuickproPos.Services
{
    public class UserService
    {
        private readonly SQLiteAsyncConnection _database;
        private const string LoggedInKey = "logged-in";
        private readonly IPreferences _preferences;

        public UserService(SQLiteAsyncConnection database, IPreferences preferences)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public bool IsSignedIn => _preferences.ContainsKey(LoggedInKey);

        public async Task<UserMaster?> LoginUser(string email, string password)
        {
            var user = await _database.Table<UserMaster>().FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _preferences.Set(LoggedInKey, user.UserId); // Save user ID as logged-in state
                return user;
            }
            return null;
        }

        public void Logout()
        {
            if (_preferences.ContainsKey(LoggedInKey))
            {
                _preferences.Remove(LoggedInKey); // Clear logged-in state
            }
        }

        public async Task<UserMaster?> GetCurrentUser()
        {
            if (!IsSignedIn)
            {
                return null;
            }

            int userId = _preferences.Get<int>(LoggedInKey, -1); // Get user ID from preferences
            return await GetUserMasterByIdAsync(userId);
        }

        public async Task<List<UserMaster>> GetAllUserMastersAsync()
        {
            return await _database.Table<UserMaster>().ToListAsync();
        }

        public async Task<UserMaster> GetUserMasterByIdAsync(int id)
        {
            return await _database.FindAsync<UserMaster>(id);
        }
        public async Task<UserMaster> GetByEmailIdAsync(string email)
        {
            var userMasters = await _database.Table<UserMaster>()
                .FirstOrDefaultAsync(u => u.Email == email);
                return userMasters;

        }
        public async Task<int> AddUserMasterAsync(UserMaster UserMaster)
        {
            var existingUserMaster = await _database.Table<UserMaster>()
                .FirstOrDefaultAsync(u => u.Email == UserMaster.Email);

            if (existingUserMaster != null)
            {
                return 0;
            }

            UserMaster.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(UserMaster);
        }

        public async Task<bool> UpdateUserMasterAsync(UserMaster UserMaster)
        {
            var existingUserMaster = await _database.Table<UserMaster>()
                .FirstOrDefaultAsync(u => u.Email == UserMaster.Email && u.UserId != UserMaster.UserId);

            if (existingUserMaster != null)
            {
                return false;
            }

            await _database.UpdateAsync(UserMaster);
            return true;
        }

        public async Task<bool> DeleteUserMasterAsync(int id)
        {
            try
            {
                await _database.DeleteAsync<UserMaster>(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting UserMaster: {ex.Message}");
                throw;
            }
        }
    }
}
