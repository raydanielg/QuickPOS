using QuickproPos.Data.Account;
using QuickproPos.Data.AccountView;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class ReportService
    {
        private readonly SQLiteAsyncConnection _database;

        public ReportService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<PurchaseMasterView>> PurchaseBySupplierAsync(DateTime fromDate, DateTime toDate, int ledgerId)
        {
                // Step 1: Filter PurchaseMaster by Date
                var purchaseMasters = await _database
        .Table<PurchaseMaster>()
        .Where(p => p.Date >= fromDate && p.Date <= toDate)
        .ToListAsync();




                // Step 2: Additional Filters for LedgerId
                if (ledgerId > 0)
                {
                    purchaseMasters = purchaseMasters.Where(p => p.LedgerId == ledgerId).ToList();
                }
                // Step 3: Load Related Data from InvoiceSetting and AccountLedger
                var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
                var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

                // Step 4: Join Data Manually in Memory
                var result = (from pm in purchaseMasters
                              join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                              join al in accountLedgers on pm.LedgerId equals al.LedgerId
                              select new PurchaseMasterView
                              {
                                  PurchaseMasterId = pm.PurchaseMasterId,
                                  VoucherNo = pm.VoucherNo,
                                  Date = pm.Date,
                                  Reference = pm.Reference,
                                  GrandTotal = pm.GrandTotal,
                                  BillDiscount = pm.BillDiscount,
                                  TotalTax = pm.TotalTax,
                                  UserId = pm.UserId,
                                  Narration = pm.Narration,
                                  Status = pm.Status,
                                  VoucherTypeId = pm.VoucherTypeId,
                                  VoucherTypeName = iset.VoucherTypeName,
                                  LedgerName = al.LedgerName
                              }).ToList();

                return result;
            }
        public async Task<List<ProductView>> ItemWisePurchaseAsync(DateTime fromDate, DateTime toDate, int productId, int warehouseId)
        {
            // Step 1: Fetch PurchaseMasters within the date range
            var purchaseMasters = await _database.Table<PurchaseMaster>()
                .Where(pm => pm.Date >= fromDate && pm.Date <= toDate)
                .ToListAsync();

            // Step 2: Filter by WarehouseId if specified
            if (warehouseId > 0)
            {
                purchaseMasters = purchaseMasters.Where(pm => pm.WarehouseId == warehouseId).ToList();
            }

            // Step 3: Fetch related PurchaseDetails and Products
            var purchaseDetails = await _database.Table<PurchaseDetails>()
                .ToListAsync();

            var products = await _database.Table<Product>().ToListAsync();

            // Step 4: Filter PurchaseDetails by ProductId if specified
            if (productId > 0)
            {
                purchaseDetails = purchaseDetails.Where(pd => pd.ProductId == productId).ToList();
            }

            // Step 5: Join and Aggregate Data
            var result = (from pd in purchaseDetails
                          join pm in purchaseMasters on pd.PurchaseMasterId equals pm.PurchaseMasterId
                          join p in products on pd.ProductId equals p.ProductId
                          group new { pd, pm, p } by new { p.ProductId, p.ProductName, p.ProductCode, pm.Date, pm.WarehouseId } into g
                          select new ProductView
                          {
                              ProductId = g.Key.ProductId,
                              ProductName = g.Key.ProductName,
                              ProductCode = g.Key.ProductCode,
                              Date = g.Key.Date,
                              WarehouseId = g.Key.WarehouseId,
                              Qty = g.Sum(x => x.pd.Qty),
                              GrossAmount = g.Sum(x => x.pd.GrossAmount),
                              DiscountAmount = g.Sum(x => x.pd.DiscountAmount),
                              TotalAmount = g.Sum(x => x.pd.Amount)
                          }).ToList();

            return result;
        }
        public async Task<List<SalesMasterView>> SalesByCustomerAsync(DateTime fromDate, DateTime toDate, int ledgerId)
        {
            // Step 1: Filter SalesMaster by Date
            var salesMasters = await _database
    .Table<SalesMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            // Step 2: Additional Filters for LedgerId
            if (ledgerId > 0)
            {
                salesMasters = salesMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }
            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in salesMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              VoucherTypeName = iset.VoucherTypeName,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<List<ProductView>> ItemWiseSalesAsync(DateTime fromDate, DateTime toDate, int productId, int warehouseId)
        {
            // Step 1: Fetch SalesMasters within the date range
            var salesMasters = await _database.Table<SalesMaster>()
                .Where(pm => pm.Date >= fromDate && pm.Date <= toDate)
                .ToListAsync();

            // Step 2: Filter by WarehouseId if specified
            if (warehouseId > 0)
            {
                salesMasters = salesMasters.Where(pm => pm.WarehouseId == warehouseId).ToList();
            }

            // Step 3: Fetch related SalesDetails and Products
            var salesDetails = await _database.Table<SalesDetails>()
                .ToListAsync();

            var products = await _database.Table<Product>().ToListAsync();

            // Step 4: Filter PurchaseDetails by ProductId if specified
            if (productId > 0)
            {
                salesDetails = salesDetails.Where(pd => pd.ProductId == productId).ToList();
            }

            // Step 5: Join and Aggregate Data
            var result = (from pd in salesDetails
                          join pm in salesMasters on pd.SalesMasterId equals pm.SalesMasterId
                          join p in products on pd.ProductId equals p.ProductId
                          group new { pd, pm, p } by new { p.ProductId, p.ProductName, p.ProductCode, pm.Date, pm.WarehouseId } into g
                          select new ProductView
                          {
                              ProductId = g.Key.ProductId,
                              ProductName = g.Key.ProductName,
                              ProductCode = g.Key.ProductCode,
                              Date = g.Key.Date,
                              WarehouseId = g.Key.WarehouseId,
                              Qty = g.Sum(x => x.pd.Qty),
                              GrossAmount = g.Sum(x => x.pd.GrossAmount),
                              DiscountAmount = g.Sum(x => x.pd.DiscountAmount),
                              TotalAmount = g.Sum(x => x.pd.Amount)
                          }).ToList();

            return result;
        }
        public async Task<List<ProductStockView>> StockReportAsync(int groupId, int productId)
        {
            // Step 1: Query the StockPosting and related tables
            var stockPostings = await _database.Table<StockPosting>().ToListAsync();
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var products = await _database.Table<Product>().ToListAsync();
            var units = await _database.Table<Unit>().ToListAsync();
            var productGroups = await _database.Table<ProductGroup>().ToListAsync();

            // Step 2: Filter the product group and product based on parameters
            if (groupId > 0)
            {
                productGroups = productGroups.Where(pg => pg.GroupId == groupId).ToList();
            }

            if (productId > 0)
            {
                products = products.Where(p => p.ProductId == productId).ToList();
            }

            // Step 3: Perform grouping and subquery logic
            var tempData = (from sp in stockPostings
                            join p in products on sp.ProductId equals p.ProductId
                            group sp by new { p.ProductId, p.ProductCode, p.ProductName } into g
                            select new
                            {
                                g.Key.ProductId,
                                g.Key.ProductCode,
                                g.Key.ProductName,
                                PurQty = g.Sum(x => x.InwardQty),
                                SalesQty = g.Sum(x => x.OutwardQty),
                                Stock = g.Sum(x => (x.InwardQty) - (x.OutwardQty))
                            }).ToList();

            // Step 4: Join temp data with related tables
            var result = (from temp in tempData
                          join p in products on temp.ProductId equals p.ProductId
                          join u in units on p.UnitId equals u.UnitId into unitJoin
                          from unit in unitJoin.DefaultIfEmpty()
                          select new ProductStockView
                          {
                              ProductId = temp.ProductId,
                              ProductCode = temp.ProductCode,
                              ProductName = temp.ProductName,
                              UnitName = unit?.UnitName,
                              Rate = GetLatestRate(stockPostings, invoiceSettings, temp.ProductId, "Purchase Invoice", "Material Receipt") ?? p.PurchaseRate,
                              PurQty = temp.PurQty,
                              PurchaseStockBal = temp.PurQty * (GetLatestRate(stockPostings, invoiceSettings, temp.ProductId, "Purchase Invoice", "Material Receipt") ?? p.PurchaseRate),
                              SalesRate = p.SalesRate,
                              SalesQty = temp.SalesQty,
                              SalesStockBalance = temp.SalesQty * p.SalesRate,
                              Stock = temp.Stock,
                              StockValue = temp.Stock * (GetLatestRate(stockPostings, invoiceSettings, temp.ProductId, "Purchase Invoice", "Material Receipt") ?? 0)
                          }).ToList();

            return result;
        }

        // Helper Method to Get Latest Rate
        private decimal? GetLatestRate(IEnumerable<StockPosting> stockPostings, IEnumerable<InvoiceSetting> invoiceSettings, int productId, params string[] voucherTypeNames)
        {
            if (stockPostings == null || invoiceSettings == null || !stockPostings.Any() || !invoiceSettings.Any())
                return null; // Return null if input data is invalid or empty

            var latestRate = (from sp in stockPostings
                              join vt in invoiceSettings on new { sp.VoucherTypeId, sp.AgainstVoucherTypeId }
                                  equals new { VoucherTypeId = vt.VoucherTypeId, AgainstVoucherTypeId = vt.VoucherTypeId } // Match key structure
                              where sp.ProductId == productId &&
                                    voucherTypeNames.Contains(vt.VoucherTypeName)
                              orderby sp.Date descending
                              select sp.Rate).FirstOrDefault();

            return latestRate; // Return the latest rate or null if none found
        }
        public async Task<IList<LedgerSummaryView>> GetCustomerLedgerSummaryAsync(DateTime fromDate, DateTime toDate, int ledgerId = 0, int accountGroupId = -1)
        {
            // Step 1: Fetch AccountLedger data
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 2: Fetch LedgerPosting data
            var ledgerPostings = await _database.Table<LedgerPosting>().ToListAsync();

            // Step 3: Filter AccountLedger based on parameters
            var filteredLedgers = accountLedgers
                .Where(al => (ledgerId == 0 || al.LedgerId == ledgerId) &&
                             (accountGroupId == -1 || al.AccountGroupId == accountGroupId))
                .ToList();

            // Step 4: Calculate Opening, Debit, and Credit
            var ledgerSummary = from ledger in filteredLedgers
                                let opening = ledgerPostings
                                    .Where(lp => lp.LedgerId == ledger.LedgerId &&
                                                 (lp.Date < fromDate || lp.Date == fromDate)
                                                  &&
                                                 lp.VoucherTypeId == 1)
                                    .Sum(lp => lp.Debit - lp.Credit)
                                let debit = ledgerPostings
                                    .Where(lp => lp.LedgerId == ledger.LedgerId &&
                                                 lp.Date >= fromDate && lp.Date <= toDate
                                                 &&
                                                 lp.VoucherTypeId != 1)
                                    .Sum(lp => lp.Debit)
                                let credit = ledgerPostings
                                    .Where(lp => lp.LedgerId == ledger.LedgerId &&
                                                 lp.Date >= fromDate && lp.Date <= toDate
                                                 &&
                                                 lp.VoucherTypeId != 1)
                                    .Sum(lp => lp.Credit)
                                // Step 5: Calculate Opening, Closing, and Group Data
                                let closing = opening + debit - credit
                                where debit > 0 || credit > 0 || opening != 0
                                select new LedgerSummaryView
                                {
                                    LedgerId = ledger.LedgerId,
                                    LedgerName = ledger.LedgerName,
                                    SlNo = 0, // Will calculate row number later
                                    Opening = opening < 0 ? $"{Math.Abs(opening):F2}Cr" : $"{opening:F2}Dr",
                                    Op = opening,
                                    Debit = debit,
                                    Credit = credit,
                                    Closing = closing > 0 ? $"{closing:F2}Dr" : $"{Math.Abs(closing):F2}Cr",
                                    Closing1 = closing.ToString("F2")
                                };

            // Step 6: Add Row Numbers
            var result = ledgerSummary
                .OrderBy(ls => ls.LedgerId)
                .Select((ls, index) =>
                {
                    ls.SlNo = index + 1;
                    return ls;
                })
                .ToList();

            return result;
        }
        public async Task<List<DaybookReportView>> GenerateDaybookReportAsync(
    DateTime fromDate,
    DateTime toDate,
    int voucherTypeId = 0,
    int ledgerId = 0)
        {
            // Step 1: Fetch data for each type of transaction
            var receipts = await _database.Table<ReceiptMaster>()
                .Where(r => r.Date >= fromDate && r.Date <= toDate
                    && (voucherTypeId == 0 || r.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || r.LedgerId == ledgerId))
                .ToListAsync();

            var payments = await _database.Table<PaymentMaster>()
                .Where(p => p.Date >= fromDate && p.Date <= toDate
                    && (voucherTypeId == 0 || p.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || p.LedgerId == ledgerId))
                .ToListAsync();

            var sales = await _database.Table<SalesMaster>()
                .Where(s => s.Date >= fromDate && s.Date <= toDate
                    && (voucherTypeId == 0 || s.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || s.LedgerId == ledgerId))
                .ToListAsync();

            var salesReturns = await _database.Table<SalesReturnMaster>()
                .Where(s => s.Date >= fromDate && s.Date <= toDate
                    && (voucherTypeId == 0 || s.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || s.LedgerId == ledgerId))
                .ToListAsync();

            var purchases = await _database.Table<PurchaseMaster>()
                .Where(p => p.Date >= fromDate && p.Date <= toDate
                    && (voucherTypeId == 0 || p.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || p.LedgerId == ledgerId))
                .ToListAsync();

            var purchasesReturns = await _database.Table<PurchaseReturnMaster>()
                .Where(p => p.Date >= fromDate && p.Date <= toDate
                    && (voucherTypeId == 0 || p.VoucherTypeId == voucherTypeId)
                    && (ledgerId == 0 || p.LedgerId == ledgerId))
                .ToListAsync();

            // Step 2: Load related data
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 3: Transform data for each type
            var receiptResults = from r in receipts
                                 join iset in invoiceSettings on r.VoucherTypeId equals iset.VoucherTypeId
                                 join al in accountLedgers on r.LedgerId equals al.LedgerId
                                 select new DaybookReportView
                                 {
                                     Date = r.Date.ToString("dd-MM-yyyy"),
                                     VoucherTypeName = iset.VoucherTypeName,
                                     VoucherNo = r.VoucherNo,
                                     LedgerName = al.LedgerName,
                                     Debit = r.Amount,
                                     Credit = 0m,
                                     Narration = r.Narration
                                 };

            var paymentResults = from p in payments
                                 join iset in invoiceSettings on p.VoucherTypeId equals iset.VoucherTypeId
                                 join al in accountLedgers on p.LedgerId equals al.LedgerId
                                 select new DaybookReportView
                                 {
                                     Date = p.Date.ToString("dd-MM-yyyy"),
                                     VoucherTypeName = iset.VoucherTypeName,
                                     VoucherNo = p.VoucherNo,
                                     LedgerName = al.LedgerName,
                                     Debit = 0m,
                                     Credit = p.Amount,
                                     Narration = p.Narration
                                 };

            var salesResults = from s in sales
                               join iset in invoiceSettings on s.VoucherTypeId equals iset.VoucherTypeId
                               join al in accountLedgers on s.LedgerId equals al.LedgerId
                               select new DaybookReportView
                               {
                                   Date = s.Date.ToString("dd-MM-yyyy"),
                                   VoucherTypeName = iset.VoucherTypeName,
                                   VoucherNo = s.VoucherNo,
                                   LedgerName = al.LedgerName,
                                   Debit = 0m,
                                   Credit = s.GrandTotal,
                                   Narration = s.Narration
                               };

            var salesReturnResults = from s in salesReturns
                               join iset in invoiceSettings on s.VoucherTypeId equals iset.VoucherTypeId
                               join al in accountLedgers on s.LedgerId equals al.LedgerId
                               select new DaybookReportView
                               {
                                   Date = s.Date.ToString("dd-MM-yyyy"),
                                   VoucherTypeName = iset.VoucherTypeName,
                                   VoucherNo = s.VoucherNo,
                                   LedgerName = al.LedgerName,
                                   Debit = s.GrandTotal,
                                   Credit = 0m,
                                   Narration = s.Narration
                               };

            var purchaseResults = from p in purchases
                                  join iset in invoiceSettings on p.VoucherTypeId equals iset.VoucherTypeId
                                  join al in accountLedgers on p.LedgerId equals al.LedgerId
                                  select new DaybookReportView
                                  {
                                      Date = p.Date.ToString("dd-MM-yyyy"),
                                      VoucherTypeName = iset.VoucherTypeName,
                                      VoucherNo = p.VoucherNo,
                                      LedgerName = al.LedgerName,
                                      Debit = p.GrandTotal,
                                      Credit = 0m,
                                      Narration = p.Narration
                                  };

            var purchasereturnResults = from p in purchasesReturns
                                  join iset in invoiceSettings on p.VoucherTypeId equals iset.VoucherTypeId
                                  join al in accountLedgers on p.LedgerId equals al.LedgerId
                                  select new DaybookReportView
                                  {
                                      Date = p.Date.ToString("dd-MM-yyyy"),
                                      VoucherTypeName = iset.VoucherTypeName,
                                      VoucherNo = p.VoucherNo,
                                      LedgerName = al.LedgerName,
                                      Debit = p.GrandTotal,
                                      Credit = 0m,
                                      Narration = p.Narration
                                  };

            // Step 4: Combine all results
            var finalResults = receiptResults
                .Concat(paymentResults)
                .Concat(salesResults)
                .Concat(salesReturnResults)
                .Concat(purchaseResults)
                .Concat(purchasereturnResults)
                .OrderBy(r => DateTime.Parse(r.Date))
                .ToList();

            return finalResults;
        }
        public async Task<List<TrialBalanceView>> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch Account Group details (filter by Nature)
            var accountGroups = await _database.Table<AccountGroup>()
                .Where(ag => ag.Nature == "Assets" || ag.Nature == "Liabilities" || ag.Nature == "Income" || ag.Nature == "Expenses")
                .ToListAsync();

            // Step 2: Get the AccountGroupIds of the account groups we just fetched
            var accountGroupIds = accountGroups.Select(ag => ag.AccountGroupId).ToList();

            // Step 3: Fetch all AccountLedgers, but filter them on the client side
            var accountLedgerDetails = await _database.Table<AccountLedger>()
                .Where(al => accountGroupIds.Contains(al.AccountGroupId)) // Filter by the AccountGroupIds
                .ToListAsync();

            // Step 4: Fetch LedgerPostings within the specified date range
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate)
                .ToListAsync();
            //LedgerPostingOpening
            var ledgerPostingsOpening = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date < fromDate)
                .ToListAsync();

            // Step 5: Calculate the Debit, Credit, OpeningBalance, and Balance for each AccountGroup
            var trialBalanceResults = new List<TrialBalanceView>();

            foreach (var accountGroup in accountGroups)
            {
                // Get account ledgers for the current group
                var accountLedgers = accountLedgerDetails
                    .Where(al => al.AccountGroupId == accountGroup.AccountGroupId)
                    .ToList();

                // Get ledger entries for the account ledgers in the date range
                var ledgerEntries = ledgerPostings
                    .Where(lp => accountLedgers.Select(al => al.LedgerId).Contains(lp.LedgerId))
                    .ToList();

                // Debit and Credit calculation
                decimal debit = ledgerEntries.Sum(lp => lp.Debit);
                decimal credit = ledgerEntries.Sum(lp => lp.Credit);

                // Opening Balance calculation
                var openingLedgers = ledgerPostingsOpening
                    .Where(lp => accountLedgers.Select(al => al.LedgerId).Contains(lp.LedgerId))
                    .ToList();
                decimal openingBalance = openingLedgers.Sum(al => al.Debit) - openingLedgers.Sum(al => al.Credit);

                // Balance calculation (Debit - Credit + OpeningBalance)
                decimal balance = debit - credit + openingBalance;

                // Format the Opening Balance and Balance in Dr/Cr format
                string openingBalanceFormatted = openingBalance > 0 ? $"{openingBalance} Dr" : $"{-openingBalance} Cr";
                string balanceFormatted = balance > 0 ? $"{balance} Dr" : $"{-balance} Cr";

                trialBalanceResults.Add(new TrialBalanceView
                {
                    AccountGroupId = accountGroup.AccountGroupId,
                    AccountGroupName = accountGroup.AccountGroupName,
                    Debit = debit,
                    Credit = credit,
                    OpeningBalance = openingBalanceFormatted,
                    Balance = balanceFormatted,
                    OpBalance = openingBalance,
                    Balance1 = balance
                });
            }

            return trialBalanceResults;
        }
        public async Task<List<PartyLedgerView>> GetPartyWiseLedgerReportAsync(DateTime fromDate, DateTime toDate, int intLedgerId)
        {
            // Step 1: Fetch LedgerPostings within the specified date range for the specific LedgerId
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate && lp.LedgerId == intLedgerId) // Filter by LedgerId
                .ToListAsync(); // ToListAsync fetches the records as a List

            // Step 2: Fetch all Ledger details (assuming it's stored in AccountLedger table)
            var ledgerDetails = await _database.Table<AccountLedger>()
                .Where(ld => ld.LedgerId == intLedgerId) // Filter by specific LedgerId
                .ToListAsync(); // Get the Ledger details for this specific LedgerId

            // Step 3: Calculate Opening Balance for the specific LedgerId
            var openingBalanceQuery = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date < fromDate && lp.LedgerId == intLedgerId) // Get postings before the start date for specific LedgerId
                .ToListAsync(); // Fetch the records

            // Calculate the Opening Balance (Debit - Credit) for the LedgerId
            decimal openingBalance = openingBalanceQuery.Sum(lp => lp.Debit) - openingBalanceQuery.Sum(lp => lp.Credit);

            // Step 4: Group LedgerPostings by LedgerId and calculate debit, credit, and closing balance
            var partyLedgerReport = ledgerPostings
                .GroupBy(lp => lp.LedgerId)
                .Select(g =>
                {
                    var ledgerName = ledgerDetails.FirstOrDefault(ld => ld.LedgerId == g.Key)?.LedgerName; // Get Ledger name

                    // Calculate debit, credit and closing balance for the party
                    decimal debit = g.Sum(lp => lp.Debit);
                    decimal credit = g.Sum(lp => lp.Credit);
                    decimal closingBalance = openingBalance + debit - credit; // Closing balance = Opening balance + Debit - Credit

                    return new PartyLedgerView
                    {
                        LedgerName = ledgerName,
                        OpeningBalance = openingBalance,
                        Debit = debit,
                        Credit = credit,
                        ClosingBalance = closingBalance
                    };
                })
                .ToList();

            return partyLedgerReport;
        }
        public async Task<ProfitLossReportView> GetFormattedProfitLossReportAsync(DateTime fromDate, DateTime toDate)
        {
            // Fetch required data as in the existing function
            var accountGroups = await _database.Table<AccountGroup>()
                .Where(ag => ag.Nature == "Income" || ag.Nature == "Expenses")
                .ToListAsync();
            var accountGroupIds = accountGroups.Select(ag => ag.AccountGroupId).ToList();
            var accountLedgers = await _database.Table<AccountLedger>()
                .Where(al => accountGroupIds.Contains(al.AccountGroupId))
                .ToListAsync();
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate)
                .ToListAsync();

            var groupedLedgerEntries = ledgerPostings
                .GroupBy(lp => lp.LedgerId)
                .Select(g => new
                {
                    LedgerId = g.Key,
                    Debit = g.Sum(lp => lp.Debit),
                    Credit = g.Sum(lp => lp.Credit)
                })
                .ToList();

            var report = new ProfitLossReportView
            {
                Incomes = new Dictionary<string, decimal>(),
                Expenses = new Dictionary<string, decimal>(),
                NetProfit = 0
            };

            foreach (var ledgerGroup in groupedLedgerEntries)
            {
                var ledger = accountLedgers.FirstOrDefault(al => al.LedgerId == ledgerGroup.LedgerId);
                if (ledger != null)
                {
                    var accountGroup = accountGroups.FirstOrDefault(ag => ag.AccountGroupId == ledger.AccountGroupId);
                    if (accountGroup != null)
                    {
                        var amount = ledgerGroup.Debit - ledgerGroup.Credit;
                        if (accountGroup.Nature == "Income")
                        {
                            if (!report.Incomes.ContainsKey(accountGroup.AccountGroupName))
                            {
                                report.Incomes[accountGroup.AccountGroupName] = 0;
                            }
                            report.Incomes[accountGroup.AccountGroupName] += amount;
                        }
                        else if (accountGroup.Nature == "Expenses")
                        {
                            if (!report.Expenses.ContainsKey(accountGroup.AccountGroupName))
                            {
                                report.Expenses[accountGroup.AccountGroupName] = 0;
                            }
                            report.Expenses[accountGroup.AccountGroupName] += amount;
                        }
                    }
                }
            }

            // Calculate Net Profit
            var totalIncome = report.Incomes.Values.Sum();
            var totalExpenses = report.Expenses.Values.Sum();
            report.NetProfit = totalIncome - totalExpenses;

            // Return the formatted report
            return report;
        }
        public async Task<BalanceSheetReportView> GetFormattedBalanceSheetReportAsync(DateTime asOfDate)
        {
            // Fetch required data
            var accountGroups = await _database.Table<AccountGroup>()
                .Where(ag => ag.Nature == "Assets" || ag.Nature == "Liabilities" || ag.Nature == "Equity")
                .ToListAsync();

            var accountGroupIds = accountGroups.Select(ag => ag.AccountGroupId).ToList();
            var accountLedgers = await _database.Table<AccountLedger>()
                .Where(al => accountGroupIds.Contains(al.AccountGroupId))
                .ToListAsync();
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date <= asOfDate)
                .ToListAsync();

            // Group and calculate balances
            var groupedLedgerEntries = ledgerPostings
                .GroupBy(lp => lp.LedgerId)
                .Select(g => new
                {
                    LedgerId = g.Key,
                    Debit = g.Sum(lp => lp.Debit),
                    Credit = g.Sum(lp => lp.Credit)
                })
                .ToList();

            var report = new BalanceSheetReportView();

            foreach (var ledgerGroup in groupedLedgerEntries)
            {
                var ledger = accountLedgers.FirstOrDefault(al => al.LedgerId == ledgerGroup.LedgerId);
                if (ledger != null)
                {
                    var accountGroup = accountGroups.FirstOrDefault(ag => ag.AccountGroupId == ledger.AccountGroupId);
                    if (accountGroup != null)
                    {
                        var amount = ledgerGroup.Debit - ledgerGroup.Credit; // Net balance (positive for assets, negative for liabilities)

                        if (accountGroup.Nature == "Assets")
                        {
                            if (!report.Assets.ContainsKey(accountGroup.AccountGroupName))
                            {
                                report.Assets[accountGroup.AccountGroupName] = 0;
                            }
                            report.Assets[accountGroup.AccountGroupName] += amount;
                        }
                        else if (accountGroup.Nature == "Liabilities")
                        {
                            if (!report.Liabilities.ContainsKey(accountGroup.AccountGroupName))
                            {
                                report.Liabilities[accountGroup.AccountGroupName] = 0;
                            }
                            report.Liabilities[accountGroup.AccountGroupName] += amount;
                        }
                        else if (accountGroup.Nature == "Equity")
                        {
                            report.Equity += amount; // Aggregate all equity accounts
                        }
                    }
                }
            }

            return report;
        }
        public async Task<List<PurchaseMasterView>> ToPayAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch PurchaseMaster within the date range
            var purchaseMasters = await _database.Table<PurchaseMaster>()
                .Where(pm => pm.Date >= fromDate && pm.Date <= toDate)
                .ToListAsync();

            // Step 3: Fetch related AccountLedger data
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join the tables and calculate the balance due
            var result = (from pm in purchaseMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new PurchaseMasterView
                          {
                              PurchaseMasterId = pm.PurchaseMasterId,
                              VoucherNo = pm.VoucherNo,
                              LedgerName = al.LedgerName,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              Date = pm.Date
                          }).ToList();

            return result;
        }
        public async Task<List<SalesMasterView>> ToReceiveAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch SalesMaster within the date range
            var salesMasters = await _database.Table<SalesMaster>()
                .Where(pm => pm.Date >= fromDate && pm.Date <= toDate)
                .ToListAsync();

            // Step 3: Fetch related AccountLedger data
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join the tables and calculate the balance due
            var result = (from pm in salesMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              LedgerName = al.LedgerName,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              Date = pm.Date
                          }).ToList();

            return result;
        }
        public async Task<DashboardView> SalesPurchaseTotalAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch LedgerPosting records within the date range
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate)
                .ToListAsync();

            // Step 2: Fetch AccountLedger records
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 3: Join LedgerPosting with AccountLedger and calculate totals
            var result = (from lp in ledgerPostings
                          join al in accountLedgers on lp.LedgerId equals al.LedgerId
                          group new { lp, al } by al.AccountGroupId into g
                          select new
                          {
                              AccountGroupId = g.Key,
                              TotalSale = g.Key == 10
                                  ? g.Sum(x => (x.lp.Credit) - (x.lp.Debit))
                                  : 0,
                              TotalPurchase = g.Key == 11
                                  ? g.Sum(x => (x.lp.Debit) - (x.lp.Credit))
                                  : 0
                          }).ToList();

            // Step 4: Aggregate total sales and purchases
            var totalSales = result.Sum(r => r.TotalSale);
            var totalPurchases = result.Sum(r => r.TotalPurchase);

            // Step 5: Return the DashboardView object
            return new DashboardView
            {
                TotalSale = totalSales,
                TotalPurchase = totalPurchases
            };
        }
        public async Task<DashboardView> CashbankamountAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch LedgerPosting records within the date range
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate)
                .ToListAsync();

            // Step 2: Fetch AccountLedger records (only for 'Cash' and 'Bank' AccountGroups)
            var accountLedgers = await _database.Table<AccountLedger>()
                .Where(al => al.AccountGroupId == 27 || al.AccountGroupId == 28)
                .ToListAsync();

            // Step 3: Join LedgerPosting with AccountLedger and calculate balances
            var result = (from lp in ledgerPostings
                          join al in accountLedgers on lp.LedgerId equals al.LedgerId
                          group new { lp, al } by al.AccountGroupId into g
                          select new
                          {
                              AccountGroupId = g.Key,
                              CashBalance = g.Key == 27
                                  ? g.Sum(x => (x.lp.Debit) - (x.lp.Credit))
                                  : 0,
                              BankBalance = g.Key == 28
                                  ? g.Sum(x => (x.lp.Debit) - (x.lp.Credit))
                                  : 0
                          }).ToList();

            // Step 4: Aggregate the balances for Cash and Bank
            var cashBalance = result.Where(r => r.AccountGroupId == 27).Sum(r => r.CashBalance);
            var bankBalance = result.Where(r => r.AccountGroupId == 28).Sum(r => r.BankBalance);

            // Step 5: Return the DashboardView object
            return new DashboardView
            {
                CashBalance = cashBalance,
                BankBalance = bankBalance
            };
        }
        public async Task<List<DashboardView>> SalesChartAsync(DateTime fromDate, DateTime toDate)
        {
            // Step 1: Fetch LedgerPostings within the date range and with the required LedgerId
            var ledgerPostings = await _database.Table<LedgerPosting>()
                .Where(lp => lp.Date >= fromDate && lp.Date <= toDate && lp.LedgerId == 5)
                .ToListAsync();

            // Step 2: Fetch related AccountLedger data
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 3: Join tables and group by month
            var result = (from lp in ledgerPostings
                          join al in accountLedgers on lp.LedgerId equals al.LedgerId
                          group lp by new
                          {
                              Month = lp.Date.Month,
                              MonthName = lp.Date.ToString("MMMM")
                          } into grouped
                          orderby grouped.Key.Month
                          select new DashboardView
                          {
                              Month = grouped.Key.MonthName,
                              GrandTotal = grouped.Sum(lp => lp.Credit - lp.Debit)
                          }).ToList();

            return result;
        }
    }
}
