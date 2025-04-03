using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class PaymentInService
    {
        private readonly SQLiteAsyncConnection _database;

        public PaymentInService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<PaymentReceiveView>> ReceiptViewAllAsync(
    DateTime fromDate,
    DateTime toDate,
    int ledgerId,
    string strType)
        {
            // Step 1: Filter ReceiptMaster by Date
            var receiptMasters = await _database
        .Table<ReceiptMaster>()
        .Where(p => p.Date >= fromDate && p.Date <= toDate)
        .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                receiptMasters = receiptMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(strType))
            {
                receiptMasters = receiptMasters.Where(p => p.Type == strType).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in receiptMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          orderby pm.SerialNo descending
                          select new PaymentReceiveView
                          {
                              ReceiptMasterId = pm.ReceiptMasterId,
                              SalesMasterId = pm.SalesMasterId,
                              Amount = pm.Amount,
                              Date = pm.Date,
                              VoucherNo = pm.VoucherNo,
                              UserId = pm.UserId,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              CashBankid = al.LedgerId,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<int> SaveAsync(ReceiptMaster model)
        {
            await _database.InsertAsync(model);
            int id = model.ReceiptMasterId;

            if (id > 0)
            {
                //ReceiptDetails table
                foreach (var item in model.listOrder)
                {
                    //AddReceiptDetails
                    ReceiptDetails details = new ReceiptDetails();
                    if (item.LedgerId > 0)
                    {
                        details.ReceiptMasterId = id;
                        details.LedgerId = item.LedgerId;
                        details.SalesMasterId = item.SalesMasterId;
                        details.TotalAmount = item.TotalAmount;
                        details.ReceiveAmount = item.ReceiveAmount;
                        details.DueAmount = item.DueAmount;
                        await _database.InsertAsync(details);
                        int intPurchaseDId = details.ReceiptDetailsId;


                        if (item.SalesMasterId > 0)
                        {
                            var master = await _database.Table<SalesMaster>()
                                                   .FirstOrDefaultAsync(pm => pm.SalesMasterId == item.SalesMasterId);

                            decimal decPay = master.PayAmount;
                            master.PayAmount = item.ReceiveAmount + decPay;
                            master.PreviousDue = (master.GrandTotal) - (item.ReceiveAmount + decPay);
                            master.BalanceDue = (master.GrandTotal) - (item.ReceiveAmount + decPay);
                            if (master.BalanceDue == 0)
                            {
                                master.Status = "Paid";
                            }
                            else
                            {
                                master.Status = "PartialPaid";
                            }
                            await _database.UpdateAsync(master);
                        }
                        //CashAndBank
                        LedgerPosting cashPosting = new LedgerPosting();
                        cashPosting.Date = model.Date;
                        cashPosting.LedgerId = item.LedgerId;
                        cashPosting.Debit = model.Amount;
                        cashPosting.Credit = 0;
                        cashPosting.VoucherNo = model.VoucherNo;
                        cashPosting.DetailsId = id;
                        cashPosting.YearId = model.FinancialYearId;
                        cashPosting.InvoiceNo = model.VoucherNo;
                        cashPosting.VoucherTypeId = model.VoucherTypeId;
                        cashPosting.LongReference = model.Narration;
                        cashPosting.ReferenceN = model.Narration;
                        cashPosting.ChequeNo = String.Empty;
                        cashPosting.ChequeDate = String.Empty;
                        cashPosting.AddedDate = DateTime.UtcNow;
                        await _database.InsertAsync(cashPosting);
                    }
                }
                //LedgerPosting
                //Supplier
                LedgerPosting ledgerPosting = new LedgerPosting();
                ledgerPosting.Date = model.Date;
                ledgerPosting.LedgerId = model.LedgerId;
                ledgerPosting.Debit = 0;
                ledgerPosting.Credit = model.Amount;
                ledgerPosting.VoucherNo = model.VoucherNo;
                ledgerPosting.DetailsId = id;
                ledgerPosting.YearId = model.FinancialYearId;
                ledgerPosting.InvoiceNo = model.VoucherNo;
                ledgerPosting.VoucherTypeId = model.VoucherTypeId;
                ledgerPosting.LongReference = model.Narration;
                ledgerPosting.ReferenceN = model.Narration;
                ledgerPosting.ChequeNo = String.Empty;
                ledgerPosting.ChequeDate = String.Empty;
                ledgerPosting.AddedDate = DateTime.UtcNow;
                await _database.InsertAsync(ledgerPosting);
                return id;
            }
            else
            {
                return 0;
            }
        }
        public async Task<ReceiptMaster> GetReceiptByIdAsync(int id)
        {
            return await _database.FindAsync<ReceiptMaster>(id);
        }
        public async Task<IList<PaymentReceiveView>> PaymentInDetailsViewAsync(int id)
        {
            // Query ReceiptDetails filtered by ReceiptMasterId
            var receiptDetailsList = await _database.Table<ReceiptDetails>()
                                        .Where(progm => progm.ReceiptMasterId == id)
                                        .ToListAsync();

            // Query SalesMaster table
            var salesMasters = await _database.Table<SalesMaster>().ToListAsync();

            // Join ReceiptDetails with SalesMaster and map to PaymentReceiveView
            var paymentReceiveViewList = (from detail in receiptDetailsList
                                          join sales in salesMasters on detail.SalesMasterId equals sales.SalesMasterId into salesGroup
                                          from sales in salesGroup.DefaultIfEmpty() // Handle cases where no matching sales record is found
                                          select new PaymentReceiveView
                                          {
                                              ReceiptDetailsId = detail.ReceiptDetailsId,
                                              ReceiptMasterId = detail.ReceiptMasterId,
                                              Amount = detail.TotalAmount,
                                              DueAmount = detail.DueAmount,
                                              ReceiveAmount = detail.ReceiveAmount,
                                              SalesMasterId = detail.SalesMasterId,
                                              VoucherNo = sales?.VoucherNo ?? "N/A", // Default to "N/A" if sales is null
                                              Date = sales?.Date ?? DateTime.MinValue, // Default to MinValue if sales is null
                                              InvoiceAmount = sales?.GrandTotal ?? 0 // Default to 0 if sales is null
                                          }).ToList();

            return paymentReceiveViewList;
        }

        public async Task<string> GetSerialNo()
        {
            // Prepare the query
            const string query = @"
        SELECT IFNULL(MAX(CAST(SerialNo AS INTEGER) + 1), 1) AS VoucherNo
        FROM ReceiptMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<bool> DeletePaymentInAsync(PaymentReceiveView model)
        {
            try
            {
                // Step 1: Fetch and delete associated ReceiptDetails
                var ReceiptDetails = await _database.Table<ReceiptDetails>()
                                                     .Where(pd => pd.ReceiptMasterId == model.ReceiptMasterId)
                                                     .ToListAsync();

                foreach (var detail in ReceiptDetails)
                {
                    // Update SalesMaster if it exists
                    var purchase = await _database.Table<SalesMaster>()
                                                   .FirstOrDefaultAsync(pm => pm.SalesMasterId == detail.SalesMasterId);

                    if (purchase != null)
                    {
                        decimal decPay = purchase.PayAmount;
                        purchase.PayAmount -= detail.ReceiveAmount;
                        purchase.PreviousDue = purchase.GrandTotal - purchase.PayAmount;
                        purchase.BalanceDue = purchase.GrandTotal - purchase.PayAmount;

                        purchase.Status = purchase.BalanceDue == 0 ? "Paid" : "PartialPaid";

                        await _database.UpdateAsync(purchase); // Use Update instead of Insert
                    }

                    await _database.DeleteAsync(detail); // Delete ReceiptDetails record
                }

                // Step 2: Delete associated LedgerPostings
                var ledgerPostings = await _database.Table<LedgerPosting>()
                                                     .Where(lp => lp.VoucherTypeId == model.VoucherTypeId &&
                                                                  lp.DetailsId == model.ReceiptMasterId)
                                                     .ToListAsync();

                foreach (var posting in ledgerPostings)
                {
                    await _database.DeleteAsync(posting);
                }

                // Step 3: Delete the ReceiptMaster record
                var ReceiptMaster = await _database.Table<ReceiptMaster>()
                                                    .FirstOrDefaultAsync(pm => pm.ReceiptMasterId == model.ReceiptMasterId);

                if (ReceiptMaster != null)
                {
                    await _database.DeleteAsync(ReceiptMaster);
                }

                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes (optional)
                // LogError(ex, "Error in DeletePaymentOutAsync");
                throw new Exception("An error occurred while deleting the receipt details. Please try again.", ex);
            }
        }

    }
}
