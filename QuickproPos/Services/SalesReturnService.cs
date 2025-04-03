using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Services
{
    public class SalesReturnService
    {
        private readonly SQLiteAsyncConnection _database;

        public SalesReturnService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        // Method to check if a name exists in the database
        public async Task<bool> CheckNameAsync(string name)
        {
            // Query the database asynchronously
            var query = await _database.Table<SalesReturnMaster>()
                                        .Where(progm => progm.VoucherNo == name)
                                        .CountAsync();
            return query > 0;
        }
        public async Task<List<ProductView>> GetAllBySalesReturnMasterDetails(int SalesReturnMasterId)
        {
            // Query the SalesReturnDetails table filtered by SalesReturnMasterId
            var SalesReturnDetailsList = await _database.Table<SalesReturnDetails>()
                                                      .Where(x => x.SalesReturnMasterId == SalesReturnMasterId)
                                                      .ToListAsync();

            // Query the Unit table
            var units = await _database.Table<Unit>().ToListAsync();

            // Query the Product table
            var products = await _database.Table<Product>().ToListAsync();

            // Join SalesReturnDetails with Units and map to ProductView
            var productViewList = (from detail in SalesReturnDetailsList
                                   join unit in units on detail.UnitId equals unit.UnitId into unitGroup
                                   from unit in unitGroup.DefaultIfEmpty() // Handle cases where no matching unit is found
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       SalesReturnDetailsId = detail.SalesReturnDetailsId,
                                       ProductId = detail.ProductId,
                                       ProductName = product?.ProductName ?? "Unknown", // Default to "Unknown" if product is null
                                       ProductCode = product?.ProductCode ?? "N/A",   // Default to "N/A" if product is null
                                       Qty = detail.Qty,
                                       UnitId = detail.UnitId,
                                       UnitName = unit?.UnitName ?? "Unknown", // Default to "Unknown" if unit is null
                                       SalesRate = detail.Rate,
                                       Discount = detail.Discount,
                                       DiscountAmount = detail.DiscountAmount,
                                       TaxId = detail.TaxId == 0 ? 1 : detail.TaxId, // Default to 1 if TaxId is 0
                                       TaxRate = detail.TaxRate,
                                       TaxAmount = detail.TaxAmount,
                                       BatchId = detail.BatchId,
                                       GrossAmount = detail.GrossAmount,
                                       NetAmount = detail.NetAmount,
                                       Amount = detail.Amount,
                                       SalesDetailsId = detail.SalesDetailsId
                                   }).ToList();

            return productViewList;
        }

        public async Task<List<SalesReturnMasterView>>SalesReturnSearchAsync(
    DateTime fromDate,
    DateTime toDate,
    int ledgerId)
        {
            // Step 1: Filter SalesReturnMaster by Date
            var SalesReturnMasters = await _database
    .Table<SalesReturnMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                SalesReturnMasters = SalesReturnMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in SalesReturnMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          orderby pm.SerialNo descending
                          select new SalesReturnMasterView
                          {
                              SalesReturnMasterId = pm.SalesReturnMasterId,
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


        public async Task<SalesReturnMaster> GetByIdAsync(int id)
        {
            return await _database.FindAsync<SalesReturnMaster>(id);
        }
        public async Task<int> SaveAsync(SalesReturnMaster model)
        {

            try
            {
                // Insert SalesReturnMaster
                await _database.InsertAsync(model);
                int id = model.SalesReturnMasterId;

                if (id > 0)
                {
                    var ledgerPostings = new List<LedgerPosting>();
                    var stockPostings = new List<StockPosting>();
                    var SalesReturnDetails = new List<SalesReturnDetails>();

                    foreach (var item in model.listOrder)
                    {
                        // Prepare SalesReturnDetails
                        var details = new SalesReturnDetails
                        {
                            SalesReturnMasterId = model.SalesReturnMasterId,
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
                            SalesDetailsId = item.SalesDetailsId
                        };

                        SalesReturnDetails.Add(details); // Add to list for bulk insert

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
                            DetailsId = details.SalesReturnDetailsId, // Will be set after insertion
                            InvoiceNo = model.VoucherNo,
                            VoucherNo = model.VoucherNo,
                            VoucherTypeId = model.VoucherTypeId,
                            WarehouseId = model.WarehouseId,
                            StockCalculate = "Sales",
                            FinancialYearId = model.FinancialYearId,
                            AddedDate = DateTime.UtcNow
                        };

                        stockPostings.Add(stockPosting); // Add to list for bulk insert
                    }

                    // Bulk insert SalesReturnDetails and StockPostings
                    await _database.InsertAllAsync(SalesReturnDetails);
                    await _database.InsertAllAsync(stockPostings);

                    // Prepare LedgerPostings for Customer, SalesAccount, and Tax
                    ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, 0, model.GrandTotal));

                    decimal supplierAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                    ledgerPostings.Add(CreateLedgerPosting(model, 3, supplierAmount, 0)); // Assuming 3 is the Sales Account

                    if (model.TotalTax > 0)
                    {
                        ledgerPostings.Add(CreateLedgerPosting(model, 2, model.TotalTax, 0)); // Assuming 2 is the VAT account
                    }

                    // Insert all LedgerPostings in bulk
                    if (ledgerPostings.Any())
                        await _database.InsertAllAsync(ledgerPostings);

                    // Commit the transaction after all operations are successful

                    return model.SalesReturnMasterId;  // Return SalesReturnMasterId
                }
                else
                {
                    throw new Exception("Failed to insert SalesReturnMaster. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }

        public async Task<bool> UpdateAsync(SalesReturnMaster model)
        {

            try
            {
                // Update SalesReturnMaster
                await _database.UpdateAsync(model);

                // List to collect all ledger postings for batch insert
                var ledgerPostings = new List<LedgerPosting>();

                // Insert or Update SalesReturnDetails and StockPostings
                foreach (var item in model.listOrder)
                {
                    SalesReturnDetails details;
                    if (item.SalesReturnDetailsId == 0)
                    {
                        // Insert new SalesReturnDetails if the ID is 0
                        details = new SalesReturnDetails
                        {
                            SalesReturnMasterId = model.SalesReturnMasterId,
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
                            SalesDetailsId = item.SalesDetailsId
                        };

                        await _database.InsertAsync(details); // Insert SalesReturnDetails

                        // Insert corresponding StockPosting for the new SalesReturnDetails
                        await InsertStockPostingAsync(details.SalesReturnDetailsId, model, item);

                    }
                    else
                    {
                        // Update existing SalesReturnDetails
                        details = new SalesReturnDetails
                        {
                            SalesReturnDetailsId = item.SalesReturnDetailsId,
                            SalesReturnMasterId = model.SalesReturnMasterId,
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
                            SalesDetailsId = item.SalesDetailsId
                        };

                        await _database.UpdateAsync(details); // Update SalesReturnDetails

                        // Update corresponding StockPosting for the existing SalesReturnDetails
                        await UpdateStockPostingAsync(model, item);
                    }
                }

                // Delete existing LedgerPosting for the given SalesReturnMasterId
                await DeleteLedgerPostingsAsync(model.SalesReturnMasterId, model.VoucherTypeId);

                // Prepare LedgerPostings for Customer, SalesAccount, and Tax  
                ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, 0, model.GrandTotal));

                decimal salesAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                ledgerPostings.Add(CreateLedgerPosting(model, 3, salesAmount, 0)); // Assuming 3 is the Sales Account

                if (model.TotalTax > 0)
                {
                    ledgerPostings.Add(CreateLedgerPosting(model, 2, model.TotalTax, 0)); // Assuming 2 is the VAT account
                }

                // Insert all LedgerPostings in bulk
                if (ledgerPostings.Any())
                    await _database.InsertAllAsync(ledgerPostings);

                // Remove SalesReturnDetails
                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<SalesReturnDetails>(deleteItem.SalesReturnDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the SalesReturnDetails record
                    }
                }

                // Remove StockPosting
                foreach (var deleteItem in model.listDelete)
                {
                    var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && 
                                               s.VoucherNo == model.VoucherNo && 
                                               s.DetailsId == deleteItem.SalesReturnDetailsId)
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

        private async Task InsertStockPostingAsync(int SalesReturnDetailsId, SalesReturnMaster model, SalesReturnDetails item)
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
                DetailsId = SalesReturnDetailsId,
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

        private async Task UpdateStockPostingAsync(SalesReturnMaster model, SalesReturnDetails item)
        {
            var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && s.VoucherNo == model.VoucherNo && s.DetailsId == item.SalesReturnDetailsId)
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
                stockPosting.DetailsId = item.SalesReturnDetailsId;
                stockPosting.InvoiceNo = model.VoucherNo;
                stockPosting.VoucherNo = model.VoucherNo;
                stockPosting.VoucherTypeId = model.VoucherTypeId;
                stockPosting.WarehouseId = model.WarehouseId;
                stockPosting.StockCalculate = "Sales";
                stockPosting.FinancialYearId = model.FinancialYearId;
                stockPosting.AddedDate = DateTime.UtcNow;

                await _database.UpdateAsync(stockPosting); // Update StockPosting
            }
        }

        private async Task DeleteLedgerPostingsAsync(int SalesReturnMasterId, int voucherTypeId)
        {
            var ledgerPostingsToDelete = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.DetailsId == SalesReturnMasterId && lp.VoucherTypeId == voucherTypeId)
                                                         .ToListAsync();

            foreach (var ledgerPosting in ledgerPostingsToDelete)
            {
                await _database.DeleteAsync(ledgerPosting); // Delete each matching LedgerPosting
            }
        }


        private LedgerPosting CreateLedgerPosting(SalesReturnMaster model, int ledgerId, decimal debit, decimal credit)
        {
            return new LedgerPosting
            {
                Date = model.Date,
                LedgerId = ledgerId,
                Debit = debit,
                Credit = credit,
                VoucherNo = model.VoucherNo,
                DetailsId = model.SalesReturnMasterId,
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
        FROM SalesReturnMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<bool> DeleteSalesReturnAsync(SalesReturnMasterView model)
        {
            try
            {
                    // Delete LedgerPostings using LINQ
                    var ledgerPostings = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.VoucherTypeId == model.VoucherTypeId && lp.DetailsId == model.SalesReturnMasterId)
                                                         .ToListAsync();
                    if (ledgerPostings.Any())
                    {
                        foreach (var posting in ledgerPostings)
                        {
                            await _database.DeleteAsync(posting);
                        }
                    }

                    // Delete StockPostings related to SalesReturnDetails using LINQ
                    var SalesReturnDetailsList = await _database.Table<SalesReturnDetails>()
                                                              .Where(pd => pd.SalesReturnMasterId == model.SalesReturnMasterId)
                                                              .ToListAsync();

                    foreach (var detail in SalesReturnDetailsList)
                    {
                        var stockPostings = await _database.Table<StockPosting>()
                                                            .Where(sp => sp.VoucherTypeId == model.VoucherTypeId &&
                                                                        sp.VoucherNo == model.VoucherNo &&
                                                                        sp.DetailsId == detail.SalesReturnDetailsId)
                                                            .ToListAsync();

                        if (stockPostings.Any())
                        {
                            foreach (var posting in stockPostings)
                            {
                                await _database.DeleteAsync(posting);
                            }
                        }
                    }

                    // Delete SalesReturnDetails records using LINQ
                    var SalesReturnDetailsToDelete = await _database.Table<SalesReturnDetails>()
                                                                 .Where(pd => pd.SalesReturnMasterId == model.SalesReturnMasterId)
                                                                 .ToListAsync();

                    if (SalesReturnDetailsToDelete.Any())
                    {
                        foreach (var detail in SalesReturnDetailsToDelete)
                        {
                            await _database.DeleteAsync(detail);
                        }
                    }

                    // Finally, delete the SalesReturnMaster record
                    var SalesReturnMaster = await _database.Table<SalesReturnMaster>()
                                                         .FirstOrDefaultAsync(pm => pm.SalesReturnMasterId == model.SalesReturnMasterId);

                    if (SalesReturnMaster != null)
                    {
                        await _database.DeleteAsync(SalesReturnMaster);
                    }


                    return true; // Successfully deleted
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the sales invoice. Please try again.", ex);
            }
        }


    }
}

