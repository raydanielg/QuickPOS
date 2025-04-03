using Azure;
using Microsoft.Data.SqlClient;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.User;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class SalesRegisterService
    {
        private readonly SQLiteAsyncConnection _database;
        public SalesRegisterService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<SalesRegisterClosingBalanceView>> GetAll(int userId, DateTime fromDate, DateTime toDate)
        {
            // Fetch Users
            var users = await _database.Table<UserMaster>().ToListAsync();

            // Fetch SalesRegisters filtered by date range and user
            var salesRegisters = await _database.Table<SalesRegister>()
                .Where(sr => sr.OpeningTime >= fromDate && sr.OpeningTime <= toDate)
                .Where(sr => userId == 0 || sr.UserId == userId)
                .ToListAsync();

            // Fetch SalesRegisterClosingBalances
            var salesRegisterClosingBalances = await _database.Table<SalesRegisterClosingBalance>().ToListAsync();

            // Perform the join and grouping operation in memory
            var result = (from user in users
                          join sr in salesRegisters on user.UserId equals sr.UserId
                          join srcb in salesRegisterClosingBalances on sr.SalesRegisterId equals srcb.SalesRegisterId into srcbJoin
                          from srcb in srcbJoin.DefaultIfEmpty()
                          group new { sr, srcb } by new
                          {
                              user.Username,
                              sr.OpeningCashAmount,
                              sr.Status,
                              sr.OpeningTime,
                              sr.ClosingTime
                          } into grouped
                          select new SalesRegisterClosingBalanceView
                          {
                              FullName = grouped.Key.Username,
                              OpeningCashAmount = grouped.Key.OpeningCashAmount,
                              Status = grouped.Key.Status,
                              Cash = grouped.Sum(x => x.srcb?.PaymentMethodId == 1 ? x.srcb.Amount : 0),
                              Card = grouped.Sum(x => x.srcb?.PaymentMethodId == 2 ? x.srcb.Amount : 0),
                              UPI = grouped.Sum(x => x.srcb?.PaymentMethodId == 3 ? x.srcb.Amount : 0),
                              TotalSales = grouped.Sum(x =>
                                  (x.srcb?.PaymentMethodId == 1 ? x.srcb.Amount : 0) +
                                  (x.srcb?.PaymentMethodId == 2 ? x.srcb.Amount : 0) +
                                  (x.srcb?.PaymentMethodId == 3 ? x.srcb.Amount : 0)),
                              PayLater = grouped.Sum(x => x.srcb?.PayLater ?? 0),
                              CreditAppliedAmount = grouped.Sum(x => x.srcb?.CreditAppliedAmount ?? 0),
                              SalesReturnAmount = grouped.Sum(x => x.srcb?.SalesReturnAmount ?? 0),
                              ClosingAmount = grouped.Sum(x => x.srcb?.ClosingAmount ?? 0),
                              OpeningTime = grouped.Key.OpeningTime,
                              ClosingTime = grouped.Key.ClosingTime
                          }).ToList();

            return result;
        }

        public async Task<List<SalesRegisterClosingBalanceView>> CashRegisterAsync(int userId)
        {
            try
            {
                // Fetch PaymentTypes
                var paymentTypes = await _database.Table<PaymentType>().ToListAsync();

                // Fetch SalesRegisters with the required filter
                var salesRegisters = await _database.Table<SalesRegister>()
                    .Where(sr => sr.Status == "Open" && sr.UserId == userId)
                    .ToListAsync();

                // Fetch SalesRegisterClosingBalances
                var salesRegisterClosingBalances = await _database.Table<SalesRegisterClosingBalance>().ToListAsync();

                // Perform the join and group operation in memory
                var result = (from pt in paymentTypes
                              join srcb in salesRegisterClosingBalances
                              on pt.PaymentTypeId equals srcb.PaymentMethodId into srcbJoin
                              from srcb in srcbJoin.DefaultIfEmpty()
                              join sr in salesRegisters
                              on srcb?.SalesRegisterId equals sr.SalesRegisterId into srJoin
                              from sr in srJoin.DefaultIfEmpty()
                              where sr != null // Ensure valid SalesRegister match
                              group new { srcb } by new { pt.PaymentTypeId, pt.Name } into grouped
                              orderby grouped.Key.PaymentTypeId
                              select new SalesRegisterClosingBalanceView
                              {
                                  PaymentId = grouped.Key.PaymentTypeId,
                                  Name = grouped.Key.Name,
                                  Amount = grouped.Sum(x => x.srcb?.Amount ?? 0)
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching cash register data: {ex.Message}");
                return new List<SalesRegisterClosingBalanceView>();
            }
        }
        public async Task<List<SalesRegisterClosingBalanceView>> CloseReturnAsync(int userId)
        {
            // SQL query for SQLite
            string query = @"
SELECT 
    IFNULL(SUM(srcb.PayLater), 0) AS PayLater,
    IFNULL(SUM(srcb.CreditAppliedAmount), 0) AS CreditAppliedAmount,
    IFNULL(SUM(srcb.SalesReturnAmount), 0) AS SalesReturnAmount,
    IFNULL(SUM(srcb.ClosingAmount), 0) AS ClosingAmount,
    IFNULL(SUM(srcb.CashReturnAmount), 0) AS CashReturnAmount,
    IFNULL(SUM(srcb.ExpenseAmount), 0) AS ExpenseAmount,
    IFNULL(SUM(srcb.PurchasePayment), 0) AS PurchasePayment
FROM SalesRegisterClosingBalance srcb
INNER JOIN SalesRegister sr 
    ON srcb.SalesRegisterId = sr.SalesRegisterId
WHERE sr.UserId = ? AND sr.Status = 'Open'";

            try
            {
                // Execute the query and map results
                var result = await _database.QueryAsync<SalesRegisterClosingBalanceView>(query, userId);

                // If no result is returned, initialize with default values
                return result;
            }
            catch (Exception ex)
            {
                // Log and handle errors as appropriate
                Console.WriteLine($"Error fetching closing balance: {ex.Message}");
                return new List<SalesRegisterClosingBalanceView>();
            }
        }


        public async Task<SalesRegister> OpeningCashAsync(int userId)
        {
            var query = await _database.Table<SalesRegister>()
                                        .Where(progm => progm.UserId == userId && progm.Status == "Open")
                                        .FirstOrDefaultAsync();
            return query;
        }
        public async Task<bool> CheckNameId(SalesRegister model)
        {
            var query = await _database.Table<SalesRegister>()
                                        .Where(progm => progm.UserId == model.UserId && progm.Status == "Open")
                                        .CountAsync();
            if (query > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CloseBalanceAsync(SalesRegister model)
        {
            try
            {
                // Check if there are any open sales registers for the user
                var openSalesRegister = await _database.Table<SalesRegister>()
                                                        .Where(sr => sr.UserId == model.UserId && sr.Status == "Open")
                                                        .FirstOrDefaultAsync();

                if (openSalesRegister == null)
                {
                    // No open sales register found
                    return false;
                }

                // Update the sales register with the closing details
                model.SalesRegisterId = openSalesRegister.SalesRegisterId;
                model.OpeningTime = openSalesRegister.OpeningTime;
                model.ClosingTime = model.ClosingTime;
                model.OpeningCashAmount = openSalesRegister.OpeningCashAmount;
                model.ClosingCashAmount = model.ClosingCashAmount;
                model.Notes = openSalesRegister.Notes;
                await _database.UpdateAsync(model);

                // Prepare the closing balance record
                var closingBalance = new SalesRegisterClosingBalance
                {
                    SalesRegisterId = openSalesRegister.SalesRegisterId,
                    PaymentMethodId = 0,
                    Amount = 0,
                    SalesMasterId = 0,
                    PayLater = 0,
                    CreditAppliedAmount = 0,
                    SalesReturnAmount = 0,
                    CashReturnAmount = 0,
                    ClosingAmount = model.ClosingCashAmount,
                    ExpenseAmount = 0,
                    PurchasePayment = 0,
                    CashLeftinDrawer = 0,
                    PhysicalDrawer = 0
                };

                // Insert the closing balance record
                await _database.InsertAsync(closingBalance);

                // Check if the record was successfully inserted
                return closingBalance.Id > 0;
            }
            catch (Exception)
            {
                // Log the exception as needed
                return false;
            }
        }

        public async Task<int> SaveAsync(SalesRegister model)
        {
            var query = await _database.Table<SalesRegister>()
                                        .Where(progm => progm.UserId == model.UserId && progm.Status == "Open")
                                        .CountAsync();
            if (query > 0)
            {
                return 0;
            }
            else
            {
                return await _database.InsertAsync(model);
            }
        }
    }
}
