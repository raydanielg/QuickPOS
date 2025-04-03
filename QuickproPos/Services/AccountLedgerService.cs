using QuickproPos.Data.Account;
using QuickproPos.Data.AccountView;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class AccountLedgerService
    {
        private readonly SQLiteAsyncConnection _database;

        public AccountLedgerService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<int> GetSerialNo()
        {
            // Prepare the query
            const string query = @"
        SELECT IFNULL(MAX(CAST(LedgerId AS INTEGER) + 1), 1) AS LedgerId
        FROM AccountLedger";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<int>(query);

            // Convert result to string and return
            return result;
        }

        public async Task<List<AccountLedgerView>> GetAllAccountLedgersAsync()
        {
            // Fetch data from both tables
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();
            var accountGroups = await _database.Table<AccountGroup>().ToListAsync();

            // Perform the join in-memory
            var query = (from c in accountLedgers
                         join l in accountGroups on c.AccountGroupId equals l.AccountGroupId
                         select new AccountLedgerView
                         {
                             LedgerId = c.LedgerId,
                             LedgerName = c.LedgerName,
                             LedgerCode = c.LedgerCode,
                             OpeningDate = c.OpeningDate,
                             OpeningBalance = c.OpeningBalance,
                             CrOrDr = c.CrOrDr,
                             AccountGroupName = l.AccountGroupName,
                             IsDefault = c.IsDefault
                         }).ToList();

            return query;
        }

        public async Task<List<AccountLedgerView>> GetAllAccountLedgersByIdAsync(int accountGroupId)
        {
            // Fetch data from both tables
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();
            var accountGroups = await _database.Table<AccountGroup>().ToListAsync();

            // Perform the join in-memory
            var query = (from c in accountLedgers
                         join l in accountGroups on c.AccountGroupId equals l.AccountGroupId
                         where c.AccountGroupId == accountGroupId
                         select new AccountLedgerView
                         {
                             LedgerId = c.LedgerId,
                             LedgerName = c.LedgerName,
                             LedgerCode = c.LedgerCode,
                             OpeningDate = c.OpeningDate,
                             OpeningBalance = c.OpeningBalance,
                             CrOrDr = c.CrOrDr,
                             AccountGroupName = l.AccountGroupName,
                             IsDefault = c.IsDefault
                         }).ToList();

            return query;
        }


        public async Task<AccountLedger> GetAccountLedgerByIdAsync(int id)
        {
            return await _database.FindAsync<AccountLedger>(id);
        }

        public async Task<int> AddAccountLedgerAsync(AccountLedger AccountLedger)
        {
            // Check if AccountLedgerName already exists
            var existingAccountLedger = await _database.Table<AccountLedger>()
                .FirstOrDefaultAsync(u => u.LedgerName == AccountLedger.LedgerName);

            if (existingAccountLedger != null)
            {
                return 0;
            }
            else
            {
                AccountLedger.AddedDate = DateTime.UtcNow;
                await _database.InsertAsync(AccountLedger);
                int id = AccountLedger.LedgerId;
                if (AccountLedger.OpeningBalance > 0)
                {
                    //PostingOpeningBalance
                    LedgerPosting ledgerPosting = new LedgerPosting();
                    ledgerPosting.Date = DateTime.UtcNow;
                    ledgerPosting.LedgerId = id;
                    if (AccountLedger.CrOrDr == "Dr")
                    {
                        ledgerPosting.Debit = AccountLedger.OpeningBalance;
                        ledgerPosting.Credit = 0;
                    }
                    else
                    {
                        ledgerPosting.Credit = AccountLedger.OpeningBalance;
                        ledgerPosting.Debit = 0;
                    }
                    ledgerPosting.VoucherNo = id.ToString();
                    ledgerPosting.DetailsId = id;
                    ledgerPosting.YearId = 1;
                    ledgerPosting.InvoiceNo = id.ToString();
                    ledgerPosting.VoucherTypeId = 1;
                    ledgerPosting.LongReference = String.Empty;
                    ledgerPosting.ReferenceN = String.Empty;
                    ledgerPosting.ChequeNo = String.Empty;
                    ledgerPosting.ChequeDate = String.Empty;
                    ledgerPosting.AddedDate = DateTime.UtcNow;
                    await _database.InsertAsync(ledgerPosting);
                }
                return id;
            }
        }

        public async Task<bool> UpdateAccountLedgerAsync(AccountLedger AccountLedger)
        {
            // Check if AccountLedgerName already exists for a different ID
            var existingAccountLedger = await _database.Table<AccountLedger>()
                .FirstOrDefaultAsync(u => u.LedgerName == AccountLedger.LedgerName && u.LedgerId != AccountLedger.LedgerId);

            if (existingAccountLedger != null)
            {
                return false;
            }

            AccountLedger.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(AccountLedger);
            return true;
        }
        public async Task<List<AccountLedgerView>> CashOrBankAsyn()
        {
            // Fetch data from both tables
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Perform the join in-memory
            var query = (from c in accountLedgers
                         where c.AccountGroupId == 27 || c.AccountGroupId == 28
                         select new AccountLedgerView
                         {
                             LedgerId = c.LedgerId,
                             LedgerName = c.LedgerName,
                             LedgerCode = c.LedgerCode,
                             OpeningDate = c.OpeningDate,
                             OpeningBalance = c.OpeningBalance,
                             CrOrDr = c.CrOrDr
                         }).ToList();

            return query;
        }

        public async Task<bool> DeleteAccountLedgerAsync(int ledgerId)
        {
            try
            {
                // Check if the LedgerId is used in any related tables
                var isLedgerInUse = await _database.Table<SalesMaster>().CountAsync(s => s.LedgerId == ledgerId) > 0
                                   || await _database.Table<SalesReturnMaster>().CountAsync(sr => sr.LedgerId == ledgerId) > 0
                                   || await _database.Table<PurchaseMaster>().CountAsync(pm => pm.LedgerId == ledgerId) > 0
                                   || await _database.Table<PurchaseReturnMaster>().CountAsync(pr => pr.LedgerId == ledgerId) > 0
                                   || await _database.Table<PaymentMaster>().CountAsync(p => p.LedgerId == ledgerId) > 0
                                   || await _database.Table<PaymentDetails>().CountAsync(pd => pd.LedgerId == ledgerId) > 0
                                   || await _database.Table<ReceiptMaster>().CountAsync(rm => rm.LedgerId == ledgerId) > 0
                                   || await _database.Table<ReceiptDetails>().CountAsync(rd => rd.LedgerId == ledgerId) > 0
                                   || await _database.Table<LedgerPosting>().CountAsync(lp => lp.LedgerId == ledgerId) > 0;

                if (isLedgerInUse)
                {
                    // Ledger is in use, do not delete
                    return false;
                }

                // Ledger is not in use, proceed with deletion
                await _database.DeleteAsync<AccountLedger>(ledgerId);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error deleting AccountLedger: {ex.Message}");
                throw;
            }
        }
    }
}
