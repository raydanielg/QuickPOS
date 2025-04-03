using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class PaymentOutService
    {
        private readonly SQLiteAsyncConnection _database;

        public PaymentOutService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<PaymentReceiveView>> PaymentViewAllAsync(
    DateTime fromDate,
    DateTime toDate,
    int ledgerId,
    string strType)
        {
            // Step 1: Filter PaymentMaster by Date
            var paymentMasters = await _database
        .Table<PaymentMaster>()
        .Where(p => p.Date >= fromDate && p.Date <= toDate)
        .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                paymentMasters = paymentMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(strType))
            {
                paymentMasters = paymentMasters.Where(p => p.Type == strType).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in paymentMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          orderby pm.SerialNo descending
                          select new PaymentReceiveView
                          {
                              PaymentMasterId = pm.PaymentMasterId,
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
        public async Task<int> SaveAsync(PaymentMaster model)
        {
            await _database.InsertAsync(model);
            int id = model.PaymentMasterId;

            if (id > 0)
            {
                //PaymentDetails table
                foreach (var item in model.listOrder)
                {
                    //AddPaymentDetails
                    PaymentDetails details = new PaymentDetails();
                    if (item.LedgerId > 0)
                    {
                        details.PaymentMasterId = id;
                        details.LedgerId = item.LedgerId;
                        details.PurchaseMasterId = item.PurchaseMasterId;
                        details.TotalAmount = item.TotalAmount;
                        details.ReceiveAmount = item.ReceiveAmount;
                        details.DueAmount = item.DueAmount;
                        await _database.InsertAsync(details);
                        int intPurchaseDId = details.PaymentDetailsId;


                        if (item.PurchaseMasterId > 0)
                        {
                            var master = await _database.Table<PurchaseMaster>()
                                                   .FirstOrDefaultAsync(pm => pm.PurchaseMasterId == item.PurchaseMasterId);

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
                        cashPosting.Debit = 0;
                        cashPosting.Credit = model.Amount;
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
                ledgerPosting.Debit = model.Amount;
                ledgerPosting.Credit = 0;
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
        public async Task<PaymentMaster> GetPaymentByIdAsync(int id)
        {
            return await _database.FindAsync<PaymentMaster>(id);
        }
        public async Task<IList<PaymentReceiveView>> PaymentOutDetailsViewAsync(int id)
        {
            // Query ReceiptDetails filtered by PaymentMasterId
            var receiptDetailsList = await _database.Table<PaymentDetails>()
                                        .Where(progm => progm.PaymentMasterId == id)
                                        .ToListAsync();

            // Query SalesMaster table
            var salesMasters = await _database.Table<PurchaseMaster>().ToListAsync();

            // Join ReceiptDetails with SalesMaster and map to PaymentReceiveView
            var paymentReceiveViewList = (from detail in receiptDetailsList
                                          join sales in salesMasters on detail.PurchaseMasterId equals sales.PurchaseMasterId into salesGroup
                                          from sales in salesGroup.DefaultIfEmpty() // Handle cases where no matching sales record is found
                                          select new PaymentReceiveView
                                          {
                                              PaymentDetailsId = detail.PaymentDetailsId,
                                              PaymentMasterId = detail.PaymentMasterId,
                                              Amount = detail.TotalAmount,
                                              DueAmount = detail.DueAmount,
                                              ReceiveAmount = detail.ReceiveAmount,
                                              PurchaseMasterId = detail.PurchaseMasterId,
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
        FROM PaymentMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<bool> DeletePaymentOutAsync(PaymentReceiveView model)
        {
            try
            {
                // Step 1: Fetch and delete associated PaymentDetails
                var paymentDetails = await _database.Table<PaymentDetails>()
                                                     .Where(pd => pd.PaymentMasterId == model.PaymentMasterId)
                                                     .ToListAsync();

                foreach (var detail in paymentDetails)
                {
                    // Update PurchaseMaster if it exists
                    var purchase = await _database.Table<PurchaseMaster>()
                                                   .FirstOrDefaultAsync(pm => pm.PurchaseMasterId == detail.PurchaseMasterId);

                    if (purchase != null)
                    {
                        decimal decPay = purchase.PayAmount;
                        purchase.PayAmount -= detail.ReceiveAmount;
                        purchase.PreviousDue = purchase.GrandTotal - purchase.PayAmount;
                        purchase.BalanceDue = purchase.GrandTotal - purchase.PayAmount;

                        purchase.Status = purchase.BalanceDue == 0 ? "Paid" : "PartialPaid";

                        await _database.UpdateAsync(purchase); // Use Update instead of Insert
                    }

                    await _database.DeleteAsync(detail); // Delete PaymentDetails record
                }

                // Step 2: Delete associated LedgerPostings
                var ledgerPostings = await _database.Table<LedgerPosting>()
                                                     .Where(lp => lp.VoucherTypeId == model.VoucherTypeId &&
                                                                  lp.DetailsId == model.PaymentMasterId)
                                                     .ToListAsync();

                foreach (var posting in ledgerPostings)
                {
                    await _database.DeleteAsync(posting);
                }

                // Step 3: Delete the PaymentMaster record
                var paymentMaster = await _database.Table<PaymentMaster>()
                                                    .FirstOrDefaultAsync(pm => pm.PaymentMasterId == model.PaymentMasterId);

                if (paymentMaster != null)
                {
                    await _database.DeleteAsync(paymentMaster);
                }

                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes (optional)
                // LogError(ex, "Error in DeletePaymentOutAsync");
                throw new Exception("An error occurred while deleting the payment details. Please try again.", ex);
            }
        }

    }
}
