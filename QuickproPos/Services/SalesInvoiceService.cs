using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.InventoryModelView;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Services
{
    public class SalesInvoiceService
    {
        private readonly SQLiteAsyncConnection _database;

        public SalesInvoiceService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        // Method to check if a name exists in the database
        public async Task<bool> CheckNameAsync(string name)
        {
            // Query the database asynchronously
            var query = await _database.Table<SalesMaster>()
                                        .Where(progm => progm.VoucherNo == name)
                                        .CountAsync();
            return query > 0;
        }
        public async Task<List<ProductView>> GetAllBySalesMasterDetails(int SalesMasterId)
        {
            // Query the SalesDetails table filtered by SalesMasterId
            var SalesDetailsList = await _database.Table<SalesDetails>()
                                                      .Where(x => x.SalesMasterId == SalesMasterId)
                                                      .ToListAsync();

            // Query the Unit table
            var units = await _database.Table<Unit>().ToListAsync();

            // Query the Product table
            var products = await _database.Table<Product>().ToListAsync();

            // Join SalesDetails with Units and map to ProductView
            var productViewList = (from detail in SalesDetailsList
                                   join unit in units on detail.UnitId equals unit.UnitId into unitGroup
                                   from unit in unitGroup.DefaultIfEmpty() // Handle cases where no matching unit is found
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       SalesDetailsId = detail.SalesDetailsId,
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
                                       OrderDetailsId = detail.OrderDetailsId
                                   }).ToList();

            return productViewList;
        }

        public async Task<List<SalesMasterView>>SalesInvoiceSearchAsync(
    DateTime fromDate,
    DateTime toDate,
    int ledgerId,
    string strStatus)
        {
            // Step 1: Filter SalesMaster by Date
            var SalesMasters = await _database
    .Table<SalesMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            // Step 2: Additional Filters for LedgerId and Status
            if (ledgerId > 0)
            {
                SalesMasters = SalesMasters.Where(p => p.LedgerId == ledgerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(strStatus) && strStatus != "All")
            {
                SalesMasters = SalesMasters.Where(p => p.Status == strStatus).ToList();
            }

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in SalesMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          where pm.PaymentStatus != "Hold" orderby pm.SerialNo descending
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              BalanceDue = pm.BalanceDue,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              UserName = pm.UserName,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              VoucherTypeName = iset.VoucherTypeName,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }

        public async Task<List<SalesMasterView>> GetHoldBills()
        {
            // Step 1: Filter SalesMaster by Date
            var SalesMasters = await _database
    .Table<SalesMaster>()
    .ToListAsync();




            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in SalesMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          where pm.PaymentStatus == "Hold"
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              BalanceDue = pm.BalanceDue,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              UserName = pm.UserName,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              PaymentStatus = pm.PaymentStatus,
                              VoucherTypeId = pm.VoucherTypeId,
                              VoucherTypeName = iset.VoucherTypeName,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }

        public async Task<List<SalesMasterView>> GetRecentTransaction()
        {
            // Step 1: Filter SalesMaster by Date
            var SalesMasters = await _database
    .Table<SalesMaster>()
    .ToListAsync();




            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in SalesMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          where pm.PaymentStatus != "Hold" orderby pm.SerialNo descending
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              BalanceDue = pm.BalanceDue,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              UserName = pm.UserName,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              PaymentStatus = pm.PaymentStatus,
                              VoucherTypeId = pm.VoucherTypeId,
                              VoucherTypeName = iset.VoucherTypeName,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }


        public async Task<SalesMaster> GetByIdAsync(int id)
        {
            return await _database.FindAsync<SalesMaster>(id);
        }
        public async Task<int> SaveAsync(SalesMaster model)
        {

            try
            {
                // Insert SalesMaster
                await _database.InsertAsync(model);
                int id = model.SalesMasterId;

                if (id > 0)
                {
                    var ledgerPostings = new List<LedgerPosting>();
                    var stockPostings = new List<StockPosting>();
                    var SalesDetails = new List<SalesDetails>();

                    foreach (var item in model.listOrder)
                    {
                        // Prepare SalesDetails
                        var details = new SalesDetails
                        {
                            SalesMasterId = model.SalesMasterId,
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

                        SalesDetails.Add(details); // Add to list for bulk insert

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
                            DetailsId = details.SalesDetailsId, // Will be set after insertion
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

                    // Bulk insert SalesDetails and StockPostings
                    await _database.InsertAllAsync(SalesDetails);
                    await _database.InsertAllAsync(stockPostings);

                    // Prepare LedgerPostings for Customer, SalesAccount, and Tax
                    ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, model.GrandTotal, 0));

                    decimal supplierAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                    ledgerPostings.Add(CreateLedgerPosting(model, 3, 0, supplierAmount)); // Assuming 3 is the Sales Account

                    if (model.TotalTax > 0)
                    {
                        ledgerPostings.Add(CreateLedgerPosting(model, 2, 0, model.TotalTax)); // Assuming 2 is the VAT account
                    }

                    // Insert all LedgerPostings in bulk
                    if (ledgerPostings.Any())
                        await _database.InsertAllAsync(ledgerPostings);

                    // Commit the transaction after all operations are successful

                    //CashRegister
                    if (model.Channel == "POS")
                    {
                        var resultSalesRegister = await _database.Table<SalesRegister>()
                                               .Where(s => s.UserId == model.UserId &&
                                               s.Status == "Open")
                                               .FirstOrDefaultAsync();
                        if (resultSalesRegister != null)
                        {
                            SalesRegisterClosingBalance closingbalance = new SalesRegisterClosingBalance();
                            closingbalance.SalesRegisterId = resultSalesRegister.SalesRegisterId;
                            closingbalance.PaymentMethodId = model.PaymentId;
                            closingbalance.Amount = model.GrandTotal;
                            closingbalance.SalesMasterId = id;
                            closingbalance.VoucherTypeId = model.VoucherTypeId;
                            closingbalance.PayLater = 0;
                            closingbalance.CreditAppliedAmount = 0;
                            closingbalance.SalesReturnAmount = 0;
                            closingbalance.CashReturnAmount = 0;
                            closingbalance.ClosingAmount = 0;
                            closingbalance.ExpenseAmount = 0;
                            closingbalance.PurchasePayment = 0;
                            closingbalance.CashLeftinDrawer = 0;
                            closingbalance.PhysicalDrawer = 0;
                            await _database.InsertAsync(closingbalance); //InsertSalesRegisterClosingbalance
                        }
                    }

                    return model.SalesMasterId;  // Return SalesMasterId
                }
                else
                {
                    throw new Exception("Failed to insert SalesMaster. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }
        public async Task<int> SaveHoldBillAsync(SalesMaster model)
        {

            try
            {
                // Insert SalesMaster
                await _database.InsertAsync(model);
                int id = model.SalesMasterId;

                if (id > 0)
                {
                    var SalesDetails = new List<SalesDetails>();

                    foreach (var item in model.listOrder)
                    {
                        // Prepare SalesDetails
                        var details = new SalesDetails
                        {
                            SalesMasterId = model.SalesMasterId,
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
                        SalesDetails.Add(details); // Add to list for bulk insert
                    }

                    // Bulk insert SalesDetails and StockPostings
                    await _database.InsertAllAsync(SalesDetails);
                    return model.SalesMasterId;  // Return SalesMasterId
                }
                else
                {
                    throw new Exception("Failed to insert hold bill. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }

        public async Task<bool> UpdateAsync(SalesMaster model)
        {

            try
            {
                // Update SalesMaster
                await _database.UpdateAsync(model);

                // List to collect all ledger postings for batch insert
                var ledgerPostings = new List<LedgerPosting>();

                // Insert or Update SalesDetails and StockPostings
                foreach (var item in model.listOrder)
                {
                    SalesDetails details;
                    if (item.SalesDetailsId == 0)
                    {
                        // Insert new SalesDetails if the ID is 0
                        details = new SalesDetails
                        {
                            SalesMasterId = model.SalesMasterId,
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

                        await _database.InsertAsync(details); // Insert SalesDetails

                        // Insert corresponding StockPosting for the new SalesDetails
                        await InsertStockPostingAsync(details.SalesDetailsId, model, item);

                    }
                    else
                    {
                        // Update existing SalesDetails
                        details = new SalesDetails
                        {
                            SalesDetailsId = item.SalesDetailsId,
                            SalesMasterId = model.SalesMasterId,
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

                        await _database.UpdateAsync(details); // Update SalesDetails

                        // Update corresponding StockPosting for the existing SalesDetails
                        await UpdateStockPostingAsync(model, item);
                    }
                }

                // Delete existing LedgerPosting for the given SalesMasterId
                await DeleteLedgerPostingsAsync(model.SalesMasterId, model.VoucherTypeId);

                // Prepare LedgerPostings for Customer, SalesAccount, and Tax  
                ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, model.GrandTotal, 0));

                decimal salesAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                ledgerPostings.Add(CreateLedgerPosting(model, 3, 0, salesAmount)); // Assuming 3 is the Sales Account

                if (model.TotalTax > 0)
                {
                    ledgerPostings.Add(CreateLedgerPosting(model, 2, 0, model.TotalTax)); // Assuming 2 is the VAT account
                }

                // Insert all LedgerPostings in bulk
                if (ledgerPostings.Any())
                    await _database.InsertAllAsync(ledgerPostings);

                // Remove SalesDetails
                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<SalesDetails>(deleteItem.SalesDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the SalesDetails record
                    }
                }

                // Remove StockPosting
                foreach (var deleteItem in model.listDelete)
                {
                    var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && 
                                               s.VoucherNo == model.VoucherNo && 
                                               s.DetailsId == deleteItem.SalesDetailsId)
                                               .FirstOrDefaultAsync();

                    if (stockPosting != null) // Ensure that a valid ID is returned
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
        public async Task<bool> UpdateHoldBillAsync(SalesMaster model)
        {

            try
            {
                // Update SalesMaster
                await _database.UpdateAsync(model);

                // List to collect all ledger postings for batch insert
                var ledgerPostings = new List<LedgerPosting>();

                // Insert or Update SalesDetails and StockPostings
                foreach (var item in model.listOrder)
                {
                    SalesDetails details;
                    if (item.SalesDetailsId == 0)
                    {
                        // Insert new SalesDetails if the ID is 0
                        details = new SalesDetails
                        {
                            SalesMasterId = model.SalesMasterId,
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

                        await _database.InsertAsync(details); // Insert SalesDetails
                        await InsertStockPostingAsync(details.SalesDetailsId, model, item);


                    }
                    else
                    {
                        // Update existing SalesDetails
                        details = new SalesDetails
                        {
                            SalesDetailsId = item.SalesDetailsId,
                            SalesMasterId = model.SalesMasterId,
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

                        await _database.UpdateAsync(details); // Update SalesDetails

                        // Insert corresponding StockPosting for the new SalesDetails
                        await InsertStockPostingAsync(details.SalesDetailsId, model, item);
                    }
                }

                // Delete existing LedgerPosting for the given SalesMasterId
                await DeleteLedgerPostingsAsync(model.SalesMasterId, model.VoucherTypeId);

                // Prepare LedgerPostings for Customer, SalesAccount, and Tax  
                ledgerPostings.Add(CreateLedgerPosting(model, model.LedgerId, model.GrandTotal, 0));

                decimal salesAmount = Math.Round(model.GrandTotal - model.TotalTax, 2);
                ledgerPostings.Add(CreateLedgerPosting(model, 3, 0, salesAmount)); // Assuming 3 is the Sales Account

                if (model.TotalTax > 0)
                {
                    ledgerPostings.Add(CreateLedgerPosting(model, 2, 0, model.TotalTax)); // Assuming 2 is the VAT account
                }

                // Insert all LedgerPostings in bulk
                if (ledgerPostings.Any())
                    await _database.InsertAllAsync(ledgerPostings);

                // Remove SalesDetails
                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<SalesDetails>(deleteItem.SalesDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the SalesDetails record
                    }
                }

                // Remove StockPosting
                foreach (var deleteItem in model.listDelete)
                {
                    var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId &&
                                               s.VoucherNo == model.VoucherNo &&
                                               s.DetailsId == deleteItem.SalesDetailsId)
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

        private async Task InsertStockPostingAsync(int SalesDetailsId, SalesMaster model, SalesDetails item)
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
                DetailsId = SalesDetailsId,
                InvoiceNo = model.VoucherNo,
                VoucherNo = model.VoucherNo,
                VoucherTypeId = model.VoucherTypeId,
                WarehouseId = model.WarehouseId,
                StockCalculate = "Sales",
                FinancialYearId = model.FinancialYearId,
                AddedDate = DateTime.UtcNow
            };

            await _database.InsertAsync(stockPosting); // Insert StockPosting
        }

        private async Task UpdateStockPostingAsync(SalesMaster model, SalesDetails item)
        {
            var stockPosting = await _database.Table<StockPosting>()
                                               .Where(s => s.VoucherTypeId == model.VoucherTypeId && s.VoucherNo == model.VoucherNo && s.DetailsId == item.SalesDetailsId)
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
                stockPosting.DetailsId = item.SalesDetailsId;
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

        private async Task DeleteLedgerPostingsAsync(int SalesMasterId, int voucherTypeId)
        {
            var ledgerPostingsToDelete = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.DetailsId == SalesMasterId && lp.VoucherTypeId == voucherTypeId)
                                                         .ToListAsync();

            foreach (var ledgerPosting in ledgerPostingsToDelete)
            {
                await _database.DeleteAsync(ledgerPosting); // Delete each matching LedgerPosting
            }
        }


        private LedgerPosting CreateLedgerPosting(SalesMaster model, int ledgerId, decimal debit, decimal credit)
        {
            return new LedgerPosting
            {
                Date = model.Date,
                LedgerId = ledgerId,
                Debit = debit,
                Credit = credit,
                VoucherNo = model.VoucherNo,
                DetailsId = model.SalesMasterId,
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
        FROM SalesMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
        public async Task<List<SalesMasterView>> PaymentInAllocationsAsync(int SalesMasterId)
        {
            var salesMasters = await _database
    .Table<SalesMaster>()
    .Where(p => p.SalesMasterId == SalesMasterId)
    .ToListAsync();
            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in salesMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          where pm.PaymentStatus != "Hold"
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              ReceiveAmount = pm.GrandTotal - pm.PayAmount,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              UserName = pm.UserName,
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<List<SalesMasterView>> PaymentInAllocationsCustomerAsync(int ledgerId)
        {
            var salesMasters = (await _database
    .Table<SalesMaster>()
    .Where(p => p.LedgerId == ledgerId)
    .ToListAsync())
    .Where(p => (p.GrandTotal - p.PayAmount) != 0)
    .ToList();

            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in salesMasters
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          where pm.PaymentStatus != "Hold"
                          select new SalesMasterView
                          {
                              SalesMasterId = pm.SalesMasterId,
                              VoucherNo = pm.VoucherNo,
                              Date = pm.Date,
                              Reference = pm.Reference,
                              GrandTotal = pm.GrandTotal,
                              ReceiveAmount = pm.GrandTotal - pm.PayAmount,
                              BalanceDue = pm.GrandTotal - pm.PayAmount,
                              BillDiscount = pm.BillDiscount,
                              TotalTax = pm.TotalTax,
                              UserId = pm.UserId,
                              UserName = pm.UserName,   
                              Narration = pm.Narration,
                              Status = pm.Status,
                              VoucherTypeId = pm.VoucherTypeId,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<bool> DeleteSalesInvoiceAsync(SalesMasterView model)
        {
            try
            {
                var isInUse = await _database.Table<ReceiptMaster>().CountAsync(s => s.SalesMasterId == model.SalesMasterId) > 0
                                   || await _database.Table<ReceiptDetails>().CountAsync(sr => sr.SalesMasterId == model.SalesMasterId) > 0;

                if (isInUse)
                {
                    return false;
                }
                else
                {
                    // Delete LedgerPostings using LINQ
                    var ledgerPostings = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.VoucherTypeId == model.VoucherTypeId && lp.DetailsId == model.SalesMasterId)
                                                         .ToListAsync();
                    if (ledgerPostings.Any())
                    {
                        foreach (var posting in ledgerPostings)
                        {
                            await _database.DeleteAsync(posting);
                        }
                    }

                    // Delete StockPostings related to SalesDetails using LINQ
                    var SalesDetailsList = await _database.Table<SalesDetails>()
                                                              .Where(pd => pd.SalesMasterId == model.SalesMasterId)
                                                              .ToListAsync();

                    foreach (var detail in SalesDetailsList)
                    {
                        var stockPostings = await _database.Table<StockPosting>()
                                                            .Where(sp => sp.VoucherTypeId == model.VoucherTypeId &&
                                                                        sp.VoucherNo == model.VoucherNo &&
                                                                        sp.DetailsId == detail.SalesDetailsId)
                                                            .ToListAsync();

                        if (stockPostings.Any())
                        {
                            foreach (var posting in stockPostings)
                            {
                                await _database.DeleteAsync(posting);
                            }
                        }
                    }

                    // Delete SalesDetails records using LINQ
                    var SalesDetailsToDelete = await _database.Table<SalesDetails>()
                                                                 .Where(pd => pd.SalesMasterId == model.SalesMasterId)
                                                                 .ToListAsync();

                    if (SalesDetailsToDelete.Any())
                    {
                        foreach (var detail in SalesDetailsToDelete)
                        {
                            await _database.DeleteAsync(detail);
                        }
                    }

                    // Finally, delete the SalesMaster record
                    var SalesMaster = await _database.Table<SalesMaster>()
                                                         .FirstOrDefaultAsync(pm => pm.SalesMasterId == model.SalesMasterId);

                    if (SalesMaster != null)
                    {
                        await _database.DeleteAsync(SalesMaster);
                    }


                    return true; // Successfully deleted
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the sales invoice. Please try again.", ex);
            }
        }


    }
}

