using QuickproPos.Data.Setting;
using SQLite;

namespace QuickproPos.Services
{
    public class CouponService
    {
        private readonly SQLiteAsyncConnection _database;

        public CouponService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<Coupon>> GetAllCouponsAsync()
        {
            return await _database.Table<Coupon>().ToListAsync();
        }

        public async Task<Coupon> GetCouponByIdAsync(int id)
        {
            return await _database.FindAsync<Coupon>(id);
        }
        public async Task<Coupon> GetCouponByCode(string code, DateTime expiryDate)
        {
            var query = await _database.Table<Coupon>()
                                        .Where(coupon => coupon.Code == code && coupon.ExpirationDate >= expiryDate)
                                        .FirstOrDefaultAsync();
            return query;
        }


        public async Task<int> AddCouponAsync(Coupon Coupon)
        {
            // Check if CouponName already exists
            var existingCoupon = await _database.Table<Coupon>()
                .FirstOrDefaultAsync(u => u.Code == Coupon.Code);

            if (existingCoupon != null)
            {
                return 0;
            }

            Coupon.AddedDate = DateTime.UtcNow;
            return await _database.InsertAsync(Coupon);
        }

        public async Task<bool> UpdateCouponAsync(Coupon Coupon)
        {
            // Check if CouponName already exists for a different ID
            var existingCoupon = await _database.Table<Coupon>()
                .FirstOrDefaultAsync(u => u.Code == Coupon.Code && u.CouponId != Coupon.CouponId);

            if (existingCoupon != null)
            {
                return false;
            }

            Coupon.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Coupon);
            return true;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            try
            {
                // Coupon is not in use, proceed with deletion
                await _database.DeleteAsync<Coupon>(id);
                return true; // Return true to indicate successful deletion
            }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error deleting Coupon: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
}


    }
}
