using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Services
{
    public class TilesQuotationService
    {
        private readonly SQLiteAsyncConnection _database;

        public TilesQuotationService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        // Method to check if a name exists in the database
        public async Task<bool> CheckNameAsync(string name)
        {
            // Query the database asynchronously
            var query = await _database.Table<TilesQuotationMaster>()
                                        .Where(progm => progm.QuotationNo == name)
                                        .CountAsync();
            return query > 0;
        }
        public async Task<List<ProductView>> GetAllByTileQuotationMasterDetails(int TileQuotationid)
        {
            var tilesDetailsList = await _database.Table<TilesQuotationDetails>()
                                                      .Where(x => x.TileQuotationid == TileQuotationid)
                                                      .ToListAsync();

            var products = await _database.Table<Product>().ToListAsync();

            var productViewList = (from detail in tilesDetailsList
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       TilesQuotatinDetailsId = detail.TilesQuotatinDetailsId,
                                       ProductId = detail.ProductId,
                                       ProductName = product?.ProductName ?? "Unknown", // Default to "Unknown" if product is null
                                       ProductCode = product?.ProductCode ?? "N/A",   // Default to "N/A" if product is null
                                       Qty = detail.Qty,
                                       UnitId = detail.UnitId,
                                       SalesRate = detail.Rate,
                                       Rate = detail.Rate,
                                       SizeLength = detail.SizeLength,
                                       SizeWidth = detail.SizeWidth,
                                       TotalSqFt = detail.TotalSqFt,
                                       TilesSizes = detail.TilesSizes,
                                       BoxQty = detail.BoxQty,
                                       Amount = detail.Amount,
                                       Description = detail.Description
                                   }).ToList();

            return productViewList;
        }
        public async Task<List<ProductView>> GetAllByTileQuotationToSalesDetails(int TileQuotationid)
        {
            var tilesDetailsList = await _database.Table<TilesQuotationDetails>()
                                                      .Where(x => x.TileQuotationid == TileQuotationid)
                                                      .ToListAsync();

            var units = await _database.Table<Unit>().ToListAsync();

            var products = await _database.Table<Product>().ToListAsync();

            var productViewList = (from detail in tilesDetailsList
                                   join unit in units on detail.UnitId equals unit.UnitId into unitGroup
                                   from unit in unitGroup.DefaultIfEmpty() // Handle cases where no matching unit is found
                                   join product in products on detail.ProductId equals product.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty() // Handle cases where no matching product is found
                                   select new ProductView
                                   {
                                       TilesQuotatinDetailsId = detail.TilesQuotatinDetailsId,
                                       ProductId = detail.ProductId,
                                       ProductName = product?.ProductName ?? "Unknown", // Default to "Unknown" if product is null
                                       ProductCode = product?.ProductCode ?? "N/A",   // Default to "N/A" if product is null
                                       Qty = detail.BoxQty,
                                       UnitId = detail.UnitId,
                                       UnitName = unit?.UnitName ?? "Unknown", // Default to "Unknown" if unit is null
                                       SalesRate = detail.Rate,
                                       TaxId = product.TaxId,
                                       Amount = detail.Amount
                                   }).ToList();

            return productViewList;
        }
        public async Task<List<TilesQuotationMasterView>> TileQuotationSearchAsync(DateTime fromDate, DateTime toDate)
        {
            var quotationMasters = await _database
    .Table<TilesQuotationMaster>()
    .Where(p => p.Date >= fromDate && p.Date <= toDate)
    .ToListAsync();




            
            // Step 3: Load Related Data from InvoiceSetting and AccountLedger
            var invoiceSettings = await _database.Table<InvoiceSetting>().ToListAsync();
            var accountLedgers = await _database.Table<AccountLedger>().ToListAsync();

            // Step 4: Join Data Manually in Memory
            var result = (from pm in quotationMasters
                          join iset in invoiceSettings on pm.VoucherTypeId equals iset.VoucherTypeId
                          join al in accountLedgers on pm.LedgerId equals al.LedgerId
                          select new TilesQuotationMasterView
                          {
                              TileQuotationid = pm.TileQuotationid,
                              QuotationNo = pm.QuotationNo,
                              Date = pm.Date,
                              TotalSqft = pm.TotalSqft,
                              TotalBox = pm.TotalBox,
                              DiscountAmount = pm.DiscountAmount,
                              TotalAmount = pm.TotalAmount,
                              AdvanceAmount = pm.AdvanceAmount,
                              GrandTotal = pm.GrandTotal,
                              Narration = pm.Narration,
                              VoucherTypeId = pm.VoucherTypeId,
                              VoucherTypeName = iset.VoucherTypeName,
                              LedgerName = al.LedgerName
                          }).ToList();

            return result;
        }
        public async Task<TilesQuotationMaster> GetByIdAsync(int id)
        {
            return await _database.FindAsync<TilesQuotationMaster>(id);
        }
        public async Task<int> SaveAsync(TilesQuotationMaster model)
        {

            try
            {
                await _database.InsertAsync(model);
                int id = model.TileQuotationid;

                if (id > 0)
                {
                    var ledgerPostings = new List<LedgerPosting>();
                    var stockPostings = new List<StockPosting>();
                    var tilesQuotationDetails = new List<TilesQuotationDetails>();

                    foreach (var item in model.listOrder)
                    {
                        var details = new TilesQuotationDetails
                        {
                            TileQuotationid = model.TileQuotationid,
                            ProductId = item.ProductId,
                            UnitId = item.UnitId,
                            Qty = item.Qty,
                            SizeLength = item.SizeLength,
                            SizeWidth = item.SizeWidth,
                            TotalSqFt = item.TotalSqFt,
                            TilesSizes = item.TilesSizes,
                            BoxQty = item.BoxQty,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            Description = item.Description
                        };

                        tilesQuotationDetails.Add(details); // Add to list for bulk insert
                    }

                    // Bulk insert SalesDetails and StockPostings
                    await _database.InsertAllAsync(tilesQuotationDetails);
                    return model.TileQuotationid;  // Return SalesMasterId
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
        public async Task<bool> UpdateAsync(TilesQuotationMaster model)
        {

            try
            {
                await _database.UpdateAsync(model);

                foreach (var item in model.listOrder)
                {
                    TilesQuotationDetails details;
                    if (item.TilesQuotatinDetailsId == 0)
                    {
                        details = new TilesQuotationDetails
                        {
                            TileQuotationid = model.TileQuotationid,
                            ProductId = item.ProductId,
                            UnitId = item.UnitId,
                            Qty = item.Qty,
                            SizeLength = item.SizeLength,
                            SizeWidth = item.SizeWidth,
                            TotalSqFt = item.TotalSqFt,
                            TilesSizes = item.TilesSizes,
                            BoxQty = item.BoxQty,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            Description = item.Description
                        };

                        await _database.InsertAsync(details); 


                    }
                    else
                    {
                        details = new TilesQuotationDetails
                        {
                            TilesQuotatinDetailsId = item.TilesQuotatinDetailsId,
                            TileQuotationid = model.TileQuotationid,
                            ProductId = item.ProductId,
                            UnitId = item.UnitId,
                            Qty = item.Qty,
                            SizeLength = item.SizeLength,
                            SizeWidth = item.SizeWidth,
                            TotalSqFt = item.TotalSqFt,
                            TilesSizes = item.TilesSizes,
                            BoxQty = item.BoxQty,
                            Rate = item.Rate,
                            Amount = item.Amount,
                            Description = item.Description
                        };

                        await _database.UpdateAsync(details);
                    }
                }

                foreach (var deleteItem in model.listDelete)
                {
                    var purchaseDetail = await _database.FindAsync<TilesQuotationDetails>(deleteItem.TilesQuotatinDetailsId);
                    if (purchaseDetail != null)
                    {
                        await _database.DeleteAsync(purchaseDetail); // Deletes the SalesDetails record
                    }
                }
                return true;  // Successfully updated
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                throw new Exception("An error occurred while saving the data. Please try again.", ex);
            }
        }
        public async Task<bool> DeleteTilesQuotationAsync(TilesQuotationMasterView model)
        {
            try
            {
                    // Delete LedgerPostings using LINQ
                    var ledgerPostings = await _database.Table<LedgerPosting>()
                                                         .Where(lp => lp.VoucherTypeId == model.VoucherTypeId && lp.DetailsId == model.TileQuotationid)
                                                         .ToListAsync();
                    if (ledgerPostings.Any())
                    {
                        foreach (var posting in ledgerPostings)
                        {
                            await _database.DeleteAsync(posting);
                        }
                    }

                    var SalesDetailsList = await _database.Table<TilesQuotationDetails>()
                                                              .Where(pd => pd.TileQuotationid == model.TileQuotationid)
                                                              .ToListAsync();

                    foreach (var detail in SalesDetailsList)
                    {
                        var stockPostings = await _database.Table<StockPosting>()
                                                            .Where(sp => sp.VoucherTypeId == model.VoucherTypeId &&
                                                                        sp.VoucherNo == model.QuotationNo &&
                                                                        sp.DetailsId == detail.TilesQuotatinDetailsId)
                                                            .ToListAsync();

                        if (stockPostings.Any())
                        {
                            foreach (var posting in stockPostings)
                            {
                                await _database.DeleteAsync(posting);
                            }
                        }
                    }

                    var SalesDetailsToDelete = await _database.Table<TilesQuotationDetails>()
                                                                 .Where(pd => pd.TileQuotationid == model.TileQuotationid)
                                                                 .ToListAsync();

                    if (SalesDetailsToDelete.Any())
                    {
                        foreach (var detail in SalesDetailsToDelete)
                        {
                            await _database.DeleteAsync(detail);
                        }
                    }

                    var SalesMaster = await _database.Table<TilesQuotationMaster>()
                                                         .FirstOrDefaultAsync(pm => pm.TileQuotationid == model.TileQuotationid);

                    if (SalesMaster != null)
                    {
                        await _database.DeleteAsync(SalesMaster);
                    }


                    return true; // Successfully deleted
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the quotation. Please try again.", ex);
            }
        }
        public async Task<string> GetSerialNo()
        {
            // Prepare the query
            const string query = @"
        SELECT IFNULL(MAX(CAST(SerialNo AS INTEGER) + 1), 1) AS VoucherNo
        FROM TilesQuotationMaster";

            // Execute the query and get the result
            var result = await _database.ExecuteScalarAsync<string>(query);

            // Convert result to string and return
            return result;
        }
    }
}
