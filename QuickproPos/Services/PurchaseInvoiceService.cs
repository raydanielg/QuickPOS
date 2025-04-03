using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;
using System.Data;

namespace QuickproPos.Services
{
    public class PurchaseInvoiceService
    {
        private readonly SQLiteAsyncConnection _database;

        public PurchaseInvoiceService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        // Method to check if a name exists in the database
        public async Task<bool> CheckNameAsync(string name)
        {
            // Query the database asynchronously
            var query = await _database.Table<PurchaseMaster>()
                                        .Where(progm => progm.VoucherNo == name)
                                        .CountAsync();
            return query > 0;
        }
        public async Task<List<ProductView>> GetAllByPurchaseMasterDetails(int purchaseMasterId)
        {
            // Query the PurchaseDetails table filtered by PurchaseMasterId
            var purchaseDetailsList = await _database.Table<PurchaseDetails>()
                                                      .Where(x => x.PurchaseMasterId == purchaseMasterId)
                                                      .ToListAsync();

            // Query the Unit table
            var units = await _database.Table<Unit>().ToListAsync();

            // Query the Product table
            var products = await _database.Table<Product>().ToListAsync();

            // Join PurchaseDetails with Units and Products and map to ProductView
            var productViewList = (from detail in purchaseDetailsList
                                   join unit in units on detail.UnitId equals unit.UnitId into unitGroup
                                   from unit in unitGroup.DefaultIfEmpty() // Handle cases where no matching unit is found
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       PurchaseDetailsId = detail.PurchaseDetailsId,
                                       ProductId = detail.ProductId,
                                       ProductName = product?.ProductName ?? "Unknown", // Default to "Unknown" if product is null
                                       ProductCode = product?.ProductCode ?? "N/A",   // Default to "N/A" if product is null
                                       Qty = detail.Qty,
                                       UnitId = detail.UnitId,
                                       UnitName = unit?.UnitName ?? "Unknown", // Default to "Unknown" if unit is null
                                       PurchaseRate = detail.Rate,
                                       Discount = detail.Discount,
                                       DiscountAmount = detail.DiscountAmount,
                                       TaxId = detail.TaxId == 0 ? 1 : detail.TaxId, // Default to 1 if TaxId is 0
                                       TaxRate = detail.TaxRate,
                                       TaxAmount = detail.TaxAmount,
                                       BatchId = detail.BatchId,
                                       GrossAmount = detail.GrossAmount,
                                       NetAmount = detail.NetAmount,
                                       Amount = detail.Amount,
                                       OrderDetailsId = detail.OrderDetailsId
                                   }).ToList();

            return productViewList;
        }


        public async Task<List<PurchaseMasterView>> GetAllAsync()
        {
            var query = @"
            SELECT 
                a.PurchaseMasterId,
                a.VoucherNo,
                a.Date,
                a.Reference,
                a.GrandTotal,
                a.BillDiscount,
                a.TotalTax,
                a.UserId,
                a.Narration,
                a.Status,
                b.VoucherTypeName,
                c.LedgerName
            FROM 
                PurchaseMaster a
            INNER JOIN 
                InvoiceSetting b ON a.VoucherTypeId = b.VoucherTypeId
            INNER JOIN 
                AccountLedger c ON a.LedgerId = c.LedgerId;"; // Replace "Ledger" with the correct table name

            // Execute the query and map the results to the PurchaseMasterView model
            var result = await _database.QueryAsync<PurchaseMasterView>(query);
            return result;
        }
        public async Task<List<PurchaseMasterView>> PurchaseInvoiceSearchAsync(DateTime fromDate,DateTime toDate,
    int ledgerId,
    string strStatus)
        {
            // Step 1: Filter PurchaseMaster by Date
            var purchaseMasters = await _database
    .Table<PurchaseMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                purchaseMasters = purchaseMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(strStatus) && strStatus != "All")
            {
                purchaseMasters = purchaseMasters.Where(p => p.Status == strStatus).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in purchaseMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          orderby pm.SerialNo descending
                          select new PurchaseMasterView
                          {
                              PurchaseMasterId = pm.PurchaseMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              BalanceDue = pm.BalanceDue,
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


        public async Task<PurchaseMaster> GetByIdAsync(int id)
        {
            return await _database.FindAsync<PurchaseMaster>(id);
        }
        public async Task<int> SaveAsync(PurchaseMaster model)
        {

            try
            {
                // Insert PurchaseMaster
                await _database.InsertAsync(model);
                int id = model.PurchaseMasterId;

                if (id > 0)
                {
                    var ledgerPostings = new List<LedgerPosting>();
                    var stockPostings = new List<StockPosting>();
                    var purchaseDetails = new List<PurchaseDetails>();

                    foreach (var item in model.listOrder)
                    {
                        // Prepare PurchaseDetails
                        var details = new PurchaseDetails
                        {
                            PurchaseMasterId = model.PurchaseMasterId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            UnitId = item.UnitId,
                            BatchId = item.BatchId,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            NetAmount = item.NetAmount,
                            GrossAmount = item.GrossAmount,
                            Discount = item.Discount,
                            DiscountAmount = item.DiscountAmount,
                            TaxAmount = item.TaxAmount,
                            TaxRate = item.TaxRate,
                            TaxId = item.TaxId,
                            OrderDetailsId = item.OrderDetailsId
                        };

                        purchaseDetails.Add(details); // Add to list for bulk insert

                        // Prepare StockPosting
                        var stockPosting = new StockPosting
                        {
                            Date = model.Date,
                            ProductId = item.ProductId,
                            InwardQty = item.Qty,
                            OutwardQty = 0,
                            UnitId = item.UnitId,
                            BatchId = item.BatchId,
                            Rate = item.Rate,
                            DetailsId = details.PurchaseDetailsId, // Will be set after insertion
                            InvoiceNo = model.VoucherNo,
                            VoucherNo = model.VoucherNo,
                            VoucherTypeId = model.VoucherTypeId,
                            WarehouseId = model.WarehouseId,
                            StockCalculate = "Purchase",
                            FinancialYearId = model.FinancialYearId,
                            AddedDate = DateTime.UtcNow
                        };

                        stockPostings.Add(stockPosting); // Add to list for bulk insert
                    }

                    // Bulk insert PurchaseDetails and StockPostings
                    await _database.InsertAllAsync(purchaseDetails);
                    await _database.InsertAllAsync(stockPostings);

                    // Prepare LedgerPostings for Supplier, PurchaseAccount, and Tax
                    ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, 0, model.GrandTotal));

                    decimal supplierAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                    ledgerPostings.Add(CreateLedgerPosting(model, 4, supplierAmount, 0)); // Assuming 4 is the Purchase Account

                    if (model.TotalTax > 0)
                    {
                        ledgerPostings.Add(CreateLedgerPosting(model, 2, model.TotalTax, 0)); // Assuming 2 is the VAT account
                    }

                    // Insert all LedgerPostings in bulk
                    if (ledgerPostings.Any())
                        await _database.InsertAllAsync(ledgerPostings);

                    // Commit the transaction after all operations are successful

                    return model.PurchaseMasterId;  // Return PurchaseMasterId
                }
                else
                {
                    throw new Exception("Failed to insert PurchaseMaster. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }

        public async Task<bool> UpdateAsync(PurchaseMaster model)
        {

            try
            {
                // Update PurchaseMaster
                await _database.UpdateAsync(model);

                // List to collect all ledger postings for batch insert
                var ledgerPostings = new List<LedgerPosting>();

                // Insert or Update PurchaseDetails and StockPostings
                foreach (var item in model.listOrder)
                {
                    PurchaseDetails details;
                    if (item.PurchaseDetailsId == 0)
                    {
                        // Insert new PurchaseDetails if the ID is 0
                        details = new PurchaseDetails
                        {
                            PurchaseMasterId = model.PurchaseMasterId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            UnitId = item.UnitId,
                            BatchId = item.BatchId,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            NetAmount = item.NetAmount,
                            GrossAmount = item.GrossAmount,
                            Discount = item.Discount,
                            DiscountAmount = item.DiscountAmount,
                            TaxAmount = item.TaxAmount,
                            TaxRate = item.TaxRate,
                            TaxId = item.TaxId,
                            OrderDetailsId = item.OrderDetailsId
                        };

                        await _database.InsertAsync(details); // Insert PurchaseDetails

                        // Insert corresponding StockPosting for the new PurchaseDetails
                        await InsertStockPostingAsync(details.PurchaseDetailsId, model, item);

                    }
                    else
                    {
                        // Update existing PurchaseDetails
                        details = new PurchaseDetails
                        {
                            PurchaseDetailsId = item.PurchaseDetailsId,
                            PurchaseMasterId = model.PurchaseMasterId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            UnitId = item.UnitId,
                            BatchId = item.BatchId,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            NetAmount = item.NetAmount,
                            GrossAmount = item.GrossAmount,
                            Discount = item.Discount,
                            DiscountAmount = item.DiscountAmount,
                            TaxAmount = item.TaxAmount,
                            TaxRate = item.TaxRate,
                            TaxId = item.TaxId,
                            OrderDetailsId = item.OrderDetailsId
                        };

                        await _database.UpdateAsync(details); // Update PurchaseDetails

                        // Update corresponding StockPosting for the existing PurchaseDetails
                        await UpdateStockPostingAsync(model, item);
                    }
                }

                // Delete existing LedgerPosting for the given PurchaseMasterId
                await DeleteLedgerPostingsAsync(model.PurchaseMasterId, model.VoucherTypeId);

                // Prepare LedgerPostings for Supplier, PurchaseAccount, and Tax
                ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, 0, model.GrandTotal));

                decimal purchaseAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                ledgerPostings.Add(CreateLedgerPosting(model, 4, purchaseAmount, 0)); // Assuming 4 is the Purchase Account

                if (model.TotalTax > 0)
                {
                    ledgerPostings.Add(CreateLedgerPosting(model, 2, model.TotalTax, 0)); // Assuming 2 is the VAT account
                }

                // Insert all LedgerPostings in bulk
                if (ledgerPostings.Any())
                    await _database.InsertAllAsync(ledgerPostings);

                // Remove PurchaseDetails
                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<PurchaseDetails>(deleteItem.PurchaseDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the PurchaseDetails record
                    }
                }

                // Remove StockPosting
                foreach (var deleteItem in model.listDelete)
                {
                    var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && 
                                               s.VoucherNo == model.VoucherNo && 
                                               s.DetailsId == deleteItem.PurchaseDetailsId)
                                               .FirstOrDefaultAsync();

                    if (stockPosting.StockPostingId != 0) // Ensure that a valid ID is returned
                    {
                        //var stockPosting = await _database.FindAsync<StockPosting>(stockPostingId.StockPostingId);
                        //if (stockPosting != null)
                        //{
                            await _database.DeleteAsync(stockPosting); // Deletes the StockPosting record
                        //}
                    }
                }

                // Commit the transaction

                return true;  // Successfully updated
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }

        private async Task InsertStockPostingAsync(int purchaseDetailsId, PurchaseMaster model, PurchaseDetails item)
        {
            var stockPosting = new StockPosting
            {
                Date = model.Date,
                ProductId = item.ProductId,
                InwardQty = item.Qty,
                OutwardQty = 0,
                UnitId = item.UnitId,
                BatchId = item.BatchId,
                Rate = item.Rate,
                DetailsId = purchaseDetailsId,
                InvoiceNo = model.VoucherNo,
                VoucherNo = model.VoucherNo,
                VoucherTypeId = model.VoucherTypeId,
                WarehouseId = model.WarehouseId,
                StockCalculate = "Purchase",
                FinancialYearId = model.FinancialYearId,
                AddedDate = DateTime.UtcNow
            };

            await _database.InsertAsync(stockPosting); // Insert StockPosting
        }

        private async Task UpdateStockPostingAsync(PurchaseMaster model, PurchaseDetails item)
        {
            var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && s.VoucherNo == model.VoucherNo && s.DetailsId == item.PurchaseDetailsId)
                                               .FirstOrDefaultAsync();

            if (stockPosting != null)
            {
                stockPosting.Date = model.Date;
                stockPosting.ProductId = item.ProductId;
                stockPosting.InwardQty = item.Qty;
                stockPosting.OutwardQty = 0;
                stockPosting.UnitId = item.UnitId;
                stockPosting.BatchId = item.BatchId;
                stockPosting.Rate = item.Rate;
                stockPosting.DetailsId = item.PurchaseDetailsId;
                stockPosting.InvoiceNo = model.VoucherNo;
                stockPosting.VoucherNo = model.VoucherNo;
                stockPosting.VoucherTypeId = model.VoucherTypeId;
                stockPosting.WarehouseId = model.WarehouseId;
                stockPosting.StockCalculate = "Purchase";
                stockPosting.FinancialYearId = model.FinancialYearId;
                stockPosting.AddedDate = DateTime.UtcNow;

                await _database.UpdateAsync(stockPosting); // Update StockPosting
            }
        }

        private async Task DeleteLedgerPostingsAsync(int purchaseMasterId, int voucherTypeId)
        {
            var ledgerPostingsToDelete = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.DetailsId == purchaseMasterId && lp.VoucherTypeId == voucherTypeId)
                                                         .ToListAsync();

            foreach (var ledgerPosting in ledgerPostingsToDelete)
            {
                await _database.DeleteAsync(ledgerPosting); // Delete each matching LedgerPosting
            }
        }


        private LedgerPosting CreateLedgerPosting(PurchaseMaster model, int ledgerId, decimal debit, decimal credit)
        {
            return new LedgerPosting
            {
                Date = model.Date,
                LedgerId = ledgerId,
                Debit = debit,
                Credit = credit,
                VoucherNo = model.VoucherNo,
                DetailsId = model.PurchaseMasterId,
                YearId = model.FinancialYearId,
                InvoiceNo = model.VoucherNo,
                VoucherTypeId = model.VoucherTypeId,
                LongReference = model.Narration,
                ReferenceN = model.Narration,
                ChequeNo = string.Empty,
                ChequeDate = string.Empty,
                AddedDate = DateTime.UtcNow
            };
        }
        public async Task<string> GetSerialNo()
        {
            // Prepare the query
            const string query = @"
        SELECT IFNULL(MAX(CAST(SerialNo AS INTEGER) + 1), 1) AS VoucherNo
        FROM PurchaseMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<List<PurchaseMasterView>> PaymentAllocationsAsync(int PurchaseMasterId)
        {
            var purchaseMasters = await _database
    .Table<PurchaseMaster>()
    .Where(p => p.PurchaseMasterId == PurchaseMasterId)
    .ToListAsync();
            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in purchaseMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new PurchaseMasterView
                          {
                              PurchaseMasterId = pm.PurchaseMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              ReceiveAmount = pm.GrandTotal - pm.PayAmount,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<List<PurchaseMasterView>> PaymentAllocationsSupplierAsync(int ledgerId)
        {
            var purchaseMasters = (await _database
    .Table<PurchaseMaster>()
    .Where(p => p.LedgerId == ledgerId)
    .ToListAsync())
    .Where(p => (p.GrandTotal - p.PayAmount) != 0)
    .ToList();

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in purchaseMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new PurchaseMasterView
                          {
                              PurchaseMasterId = pm.PurchaseMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              ReceiveAmount = pm.GrandTotal - pm.PayAmount,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<bool> DeletePurchaseInvoiceAsync(PurchaseMasterView model)
        {
            try
            {
                var isInUse = await _database.Table<PaymentMaster>().CountAsync(s => s.PurchaseMasterId == model.PurchaseMasterId) > 0
                                   || await _database.Table<PaymentDetails>().CountAsync(sr => sr.PurchaseMasterId == model.PurchaseMasterId) > 0;

                if (isInUse)
                {
                    return false;
                }
                else
                {
                    // Delete LedgerPostings using LINQ
                    var ledgerPostings = await _database.Table<LedgerPosting>()
                                                             .Where(lp => lp.VoucherTypeId == model.VoucherTypeId && lp.DetailsId == model.PurchaseMasterId)
                                                             .ToListAsync();
                    if (ledgerPostings.Any())
                    {
                        foreach (var posting in ledgerPostings)
                        {
                            await _database.DeleteAsync(posting);
                        }
                    }

                    // Delete StockPostings related to PurchaseDetails using LINQ
                    var purchaseDetailsList = await _database.Table<PurchaseDetails>()
                                                              .Where(pd => pd.PurchaseMasterId == model.PurchaseMasterId)
                                                              .ToListAsync();

                    foreach (var detail in purchaseDetailsList)
                    {
                        var stockPostings = await _database.Table<StockPosting>()
                                                            .Where(sp => sp.VoucherTypeId == model.VoucherTypeId &&
                                                                        sp.VoucherNo == model.VoucherNo &&
                                                                        sp.DetailsId == detail.PurchaseDetailsId)
                                                            .ToListAsync();

                        if (stockPostings.Any())
                        {
                            foreach (var posting in stockPostings)
                            {
                                await _database.DeleteAsync(posting);
                            }
                        }
                    }

                    // Delete PurchaseDetails records using LINQ
                    var purchaseDetailsToDelete = await _database.Table<PurchaseDetails>()
                                                                 .Where(pd => pd.PurchaseMasterId == model.PurchaseMasterId)
                                                                 .ToListAsync();

                    if (purchaseDetailsToDelete.Any())
                    {
                        foreach (var detail in purchaseDetailsToDelete)
                        {
                            await _database.DeleteAsync(detail);
                        }
                    }

                    // Finally, delete the PurchaseMaster record
                    var purchaseMaster = await _database.Table<PurchaseMaster>()
                                                         .FirstOrDefaultAsync(pm => pm.PurchaseMasterId == model.PurchaseMasterId);

                    if (purchaseMaster != null)
                    {
                        await _database.DeleteAsync(purchaseMaster);
                    }


                    return true; // Successfully deleted
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the purchase invoice. Please try again.", ex);
            }
        }


    }
}

