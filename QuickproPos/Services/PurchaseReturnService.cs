using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Services
{
    public class PurchaseReturnService
    {
        private readonly SQLiteAsyncConnection _database;

        public PurchaseReturnService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        // Method to check if a name exists in the database
        public async Task<bool> CheckNameAsync(string name)
        {
            // Query the database asynchronously
            var query = await _database.Table<PurchaseReturnMaster>()
                                        .Where(progm => progm.VoucherNo == name)
                                        .CountAsync();
            return query > 0;
        }
        public async Task<List<ProductView>> GetAllByPurchaseReturnMasterDetails(int PurchaseReturnMasterId)
        {
            // Query the PurchaseReturnDetails table filtered by PurchaseReturnMasterId
            var PurchaseReturnDetailsList = await _database.Table<PurchaseReturnDetails>()
                                                      .Where(x => x.PurchaseReturnMasterId == PurchaseReturnMasterId)
                                                      .ToListAsync();

            // Query the Unit table
            var units = await _database.Table<Unit>().ToListAsync();

            // Query the Product table
            var products = await _database.Table<Product>().ToListAsync();

            // Join PurchaseReturnDetails with Units and map to ProductView
            var productViewList = (from detail in PurchaseReturnDetailsList
                                   join unit in units on detail.UnitId equals unit.UnitId into unitGroup
                                   from unit in unitGroup.DefaultIfEmpty() // Handle cases where no matching unit is found
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       PurchaseReturnDetailsId = detail.PurchaseReturnDetailsId,
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
                                       OrderDetailsId = detail.PurchaseDetailsId
                                   }).ToList();

            return productViewList;
        }

        public async Task<List<PurchaseReturnMasterView>> GetAllAsync()
        {
            var query = @"
            SELECT 
                a.PurchaseReturnMasterId,
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
                PurchaseReturnMaster a
            INNER JOIN 
                InvoiceSetting b ON a.VoucherTypeId = b.VoucherTypeId
            INNER JOIN 
                AccountLedger c ON a.LedgerId = c.LedgerId;"; // Replace "Ledger" with the correct table name

            // Execute the query and map the results to the PurchaseReturnMasterView model
            var result = await _database.QueryAsync<PurchaseReturnMasterView>(query);
            return result;
        }
        public async Task<List<PurchaseReturnMasterView>> PurchaseReturnInvoiceSearchAsync(
    DateTime fromDate,
    DateTime toDate,
    int ledgerId)
        {
            // Step 1: Filter PurchaseReturnMaster by Date
            var PurchaseReturnMasters = await _database
    .Table<PurchaseReturnMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                PurchaseReturnMasters = PurchaseReturnMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in PurchaseReturnMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          orderby pm.SerialNo descending
                          select new PurchaseReturnMasterView
                          {
                              PurchaseReturnMasterId = pm.PurchaseReturnMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
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


        public async Task<PurchaseReturnMaster> GetByIdAsync(int id)
        {
            return await _database.FindAsync<PurchaseReturnMaster>(id);
        }
        public async Task<int> SaveAsync(PurchaseReturnMaster model)
        {

            try
            {
                // Insert PurchaseReturnMaster
                await _database.InsertAsync(model);
                int id = model.PurchaseReturnMasterId;

                if (id > 0)
                {
                    var ledgerPostings = new List<LedgerPosting>();
                    var stockPostings = new List<StockPosting>();
                    var PurchaseReturnDetails = new List<PurchaseReturnDetails>();

                    foreach (var item in model.listOrder)
                    {
                        // Prepare PurchaseReturnDetails
                        var details = new PurchaseReturnDetails
                        {
                            PurchaseReturnMasterId = model.PurchaseReturnMasterId,
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
                            PurchaseDetailsId = item.PurchaseDetailsId
                        };

                        PurchaseReturnDetails.Add(details); // Add to list for bulk insert

                        // Prepare StockPosting
                        var stockPosting = new StockPosting
                        {
                            Date = model.Date,
                            ProductId = item.ProductId,
                            InwardQty = 0,
                            OutwardQty = item.Qty,
                            UnitId = item.UnitId,
                            BatchId = item.BatchId,
                            Rate = item.Rate,
                            DetailsId = details.PurchaseReturnDetailsId, // Will be set after insertion
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

                    // Bulk insert PurchaseReturnDetails and StockPostings
                    await _database.InsertAllAsync(PurchaseReturnDetails);
                    await _database.InsertAllAsync(stockPostings);

                    // Prepare LedgerPostings for Supplier, PurchaseAccount, and Tax
                    ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, model.GrandTotal, 0));

                    decimal supplierAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                    ledgerPostings.Add(CreateLedgerPosting(model, 4, 0, supplierAmount)); // Assuming 6 is the Purchase Account

                    if (model.TotalTax > 0)
                    {
                        ledgerPostings.Add(CreateLedgerPosting(model, 2, 0, model.TotalTax)); // Assuming 2 is the VAT account
                    }

                    // Insert all LedgerPostings in bulk
                    if (ledgerPostings.Any())
                        await _database.InsertAllAsync(ledgerPostings);

                    // Commit the transaction after all operations are successful

                    return model.PurchaseReturnMasterId;  // Return PurchaseReturnMasterId
                }
                else
                {
                    throw new Exception("Failed to insert PurchaseReturnMaster. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }

        public async Task<bool> UpdateAsync(PurchaseReturnMaster model)
        {

            try
            {
                // Update PurchaseReturnMaster
                await _database.UpdateAsync(model);

                // List to collect all ledger postings for batch insert
                var ledgerPostings = new List<LedgerPosting>();

                // Insert or Update PurchaseReturnDetails and StockPostings
                foreach (var item in model.listOrder)
                {
                    PurchaseReturnDetails details;
                    if (item.PurchaseReturnDetailsId == 0)
                    {
                        // Insert new PurchaseReturnDetails if the ID is 0
                        details = new PurchaseReturnDetails
                        {
                            PurchaseReturnMasterId = model.PurchaseReturnMasterId,
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
                            PurchaseDetailsId = item.PurchaseDetailsId
                        };

                        await _database.InsertAsync(details); // Insert PurchaseReturnDetails

                        // Insert corresponding StockPosting for the new PurchaseReturnDetails
                        await InsertStockPostingAsync(details.PurchaseReturnDetailsId, model, item);

                    }
                    else
                    {
                        // Update existing PurchaseReturnDetails
                        details = new PurchaseReturnDetails
                        {
                            PurchaseReturnDetailsId = item.PurchaseReturnDetailsId,
                            PurchaseReturnMasterId = model.PurchaseReturnMasterId,
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
                            PurchaseDetailsId = item.PurchaseDetailsId
                        };

                        await _database.UpdateAsync(details); // Update PurchaseReturnDetails

                        // Update corresponding StockPosting for the existing PurchaseReturnDetails
                        await UpdateStockPostingAsync(model, item);
                    }
                }

                // Delete existing LedgerPosting for the given PurchaseReturnMasterId
                await DeleteLedgerPostingsAsync(model.PurchaseReturnMasterId, model.VoucherTypeId);

                // Prepare LedgerPostings for Supplier, PurchaseAccount, and Tax
                ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, model.GrandTotal, 0));

                decimal purchaseAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                ledgerPostings.Add(CreateLedgerPosting(model, 4, 0, purchaseAmount)); // Assuming 6 is the Purchase Account

                if (model.TotalTax > 0)
                {
                    ledgerPostings.Add(CreateLedgerPosting(model, 2, 0, model.TotalTax)); // Assuming 2 is the VAT account
                }

                // Insert all LedgerPostings in bulk
                if (ledgerPostings.Any())
                    await _database.InsertAllAsync(ledgerPostings);

                // Remove PurchaseReturnDetails
                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<PurchaseReturnDetails>(deleteItem.PurchaseReturnDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the PurchaseReturnDetails record
                    }
                }

                // Remove StockPosting
                foreach (var deleteItem in model.listDelete)
                {
                    var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && 
                                               s.VoucherNo == model.VoucherNo && 
                                               s.DetailsId == deleteItem.PurchaseReturnDetailsId)
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

        private async Task InsertStockPostingAsync(int PurchaseReturnDetailsId, PurchaseReturnMaster model, PurchaseReturnDetails item)
        {
            var stockPosting = new StockPosting
            {
                Date = model.Date,
                ProductId = item.ProductId,
                InwardQty = 0,
                OutwardQty = item.Qty,
                UnitId = item.UnitId,
                BatchId = item.BatchId,
                Rate = item.Rate,
                DetailsId = PurchaseReturnDetailsId,
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

        private async Task UpdateStockPostingAsync(PurchaseReturnMaster model, PurchaseReturnDetails item)
        {
            var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && s.VoucherNo == model.VoucherNo && s.DetailsId == item.PurchaseReturnDetailsId)
                                               .FirstOrDefaultAsync();

            if (stockPosting != null)
            {
                stockPosting.Date = model.Date;
                stockPosting.ProductId = item.ProductId;
                stockPosting.InwardQty = 0;
                stockPosting.OutwardQty = item.Qty;
                stockPosting.UnitId = item.UnitId;
                stockPosting.BatchId = item.BatchId;
                stockPosting.Rate = item.Rate;
                stockPosting.DetailsId = item.PurchaseReturnDetailsId;
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

        private async Task DeleteLedgerPostingsAsync(int PurchaseReturnMasterId, int voucherTypeId)
        {
            var ledgerPostingsToDelete = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.DetailsId == PurchaseReturnMasterId && lp.VoucherTypeId == voucherTypeId)
                                                         .ToListAsync();

            foreach (var ledgerPosting in ledgerPostingsToDelete)
            {
                await _database.DeleteAsync(ledgerPosting); // Delete each matching LedgerPosting
            }
        }


        private LedgerPosting CreateLedgerPosting(PurchaseReturnMaster model, int ledgerId, decimal debit, decimal credit)
        {
            return new LedgerPosting
            {
                Date = model.Date,
                LedgerId = ledgerId,
                Debit = debit,
                Credit = credit,
                VoucherNo = model.VoucherNo,
                DetailsId = model.PurchaseReturnMasterId,
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
        FROM PurchaseReturnMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<bool> DeletePurchaseReturnAsync(PurchaseReturnMasterView model)
        {
            try
            {
                    // Delete LedgerPostings using LINQ
                    var ledgerPostings = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.VoucherTypeId == model.VoucherTypeId && lp.DetailsId == model.PurchaseReturnMasterId)
                                                         .ToListAsync();
                    if (ledgerPostings.Any())
                    {
                        foreach (var posting in ledgerPostings)
                        {
                            await _database.DeleteAsync(posting);
                        }
                    }

                    // Delete StockPostings related to PurchaseReturnDetails using LINQ
                    var PurchaseReturnDetailsList = await _database.Table<PurchaseReturnDetails>()
                                                              .Where(pd => pd.PurchaseReturnMasterId == model.PurchaseReturnMasterId)
                                                              .ToListAsync();

                    foreach (var detail in PurchaseReturnDetailsList)
                    {
                        var stockPostings = await _database.Table<StockPosting>()
                                                            .Where(sp => sp.VoucherTypeId == model.VoucherTypeId &&
                                                                        sp.VoucherNo == model.VoucherNo &&
                                                                        sp.DetailsId == detail.PurchaseReturnDetailsId)
                                                            .ToListAsync();

                        if (stockPostings.Any())
                        {
                            foreach (var posting in stockPostings)
                            {
                                await _database.DeleteAsync(posting);
                            }
                        }
                    }

                    // Delete PurchaseReturnDetails records using LINQ
                    var PurchaseReturnDetailsToDelete = await _database.Table<PurchaseReturnDetails>()
                                                                 .Where(pd => pd.PurchaseReturnMasterId == model.PurchaseReturnMasterId)
                                                                 .ToListAsync();

                    if (PurchaseReturnDetailsToDelete.Any())
                    {
                        foreach (var detail in PurchaseReturnDetailsToDelete)
                        {
                            await _database.DeleteAsync(detail);
                        }
                    }

                    // Finally, delete the PurchaseReturnMaster record
                    var PurchaseReturnMaster = await _database.Table<PurchaseReturnMaster>()
                                                         .FirstOrDefaultAsync(pm => pm.PurchaseReturnMasterId == model.PurchaseReturnMasterId);

                    if (PurchaseReturnMaster != null)
                    {
                        await _database.DeleteAsync(PurchaseReturnMaster);
                    }


                    return true; // Successfully deleted
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the purchase return invoice. Please try again.", ex);
            }
        }


    }
}

