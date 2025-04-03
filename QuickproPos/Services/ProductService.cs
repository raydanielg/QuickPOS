using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using QuickproPos.Data.SettingView;
using SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickproPos.Services
{
    public class ProductService
    {
        private readonly SQLiteAsyncConnection _database;

        public ProductService(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public async Task<List<ProductView>> GetAllProductsAsync()
        {
            // Define the SQL query to include all required fields, including current stock and image URL
            var sqlQuery = @"
        SELECT DISTINCT 
            temp.ProductId,
            temp.ProductCode,
                temp.Barcode,
            temp.ProductName AS ProductName,
            Product.Image AS Image,
            Unit.UnitName,
            ProductGroup.GroupName,
            Product.UnitId,
            Product.TaxId,
            Tax.TaxName,
            Product.GroupId,
            Product.Includingpurchasetax,
            Product.Includingsalestax,
            CAST(Product.SalesRate AS DECIMAL(18, 2)) AS SalesRate,
CAST(Product.SalesRate AS DECIMAL(18, 2)) AS Rate,
            CAST(Product.Mrp AS DECIMAL(18, 2)) AS Mrp,
            CAST(Product.PurchaseRate AS DECIMAL(18, 2)) AS PurchaseRate,
            temp.Stock AS CurrentStock
        FROM 
        (
            SELECT 
                Product.ProductId,
                Product.ProductCode,
                Product.Barcode,
                Product.ProductName,
                CAST(
                    IFNULL(SUM(StockPosting.InwardQty), 0) - IFNULL(SUM(StockPosting.OutwardQty), 0) 
                    AS DECIMAL(18, 2)
                ) AS Stock
            FROM 
                Product
            LEFT JOIN 
                StockPosting ON Product.ProductId = StockPosting.ProductId
            GROUP BY 
                Product.ProductCode, Product.Barcode, Product.ProductName, Product.ProductId
        ) AS temp
        LEFT JOIN Product ON Product.ProductId = temp.ProductId
        LEFT JOIN Unit ON Product.UnitId = Unit.UnitId
        LEFT JOIN ProductGroup ON Product.GroupId = ProductGroup.GroupId
        LEFT JOIN Tax ON Product.TaxId = Tax.TaxId";

            // Execute the query and map results to the ProductView model
            var result = await _database.QueryAsync<ProductView>(sqlQuery);

            return result;
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _database.FindAsync<Product>(id);
        }
        public async Task<int> AddProductAsync(Product Product)
        {
            // Check if ProductName already exists
            var existingProduct = await _database.Table<Product>()
                .FirstOrDefaultAsync(u => u.ProductName == Product.ProductName);

            if (existingProduct != null)
            {
                return 0;
            }
            else
            {
                Product.AddedDate = DateTime.UtcNow;
                await _database.InsertAsync(Product);
                int id = Product.ProductId;
                //PostingOpeningStock
                if (Product.OpeningStock > 0)
                {
                    StockPosting stockposting = new StockPosting();
                    stockposting.Date = Product.ExiparyDate;
                    stockposting.ProductId = Product.ProductId;
                    stockposting.InwardQty = Product.OpeningStock;
                    stockposting.OutwardQty = 0;
                    stockposting.UnitId = Product.UnitId;
                    stockposting.BatchId = 1;
                    stockposting.Rate = Product.PurchaseRate;
                    stockposting.DetailsId = id;
                    stockposting.InvoiceNo = id.ToString();
                    stockposting.VoucherNo = id.ToString();
                    stockposting.VoucherTypeId = 2;
                    stockposting.AgainstInvoiceNo = string.Empty;
                    stockposting.AgainstVoucherNo = string.Empty;
                    stockposting.AgainstVoucherTypeId = 0;
                    stockposting.WarehouseId = 1;
                    stockposting.StockCalculate = "OpeningStock";
                    stockposting.FinancialYearId = 1;
                    stockposting.AddedDate = DateTime.UtcNow;
                    await _database.InsertAsync(stockposting);
                }
                return id;
            }
        }

        public async Task<bool> UpdateProductAsync(Product Product)
        {
            // Check if ProductName already exists for a different ID
            var existingProduct = await _database.Table<Product>()
                .FirstOrDefaultAsync(u => u.ProductName == Product.ProductName && u.ProductId != Product.ProductId);

            if (existingProduct != null)
            {
                return false;
            }

            Product.ModifyDate = DateTime.UtcNow;
            await _database.UpdateAsync(Product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                // Check if the ProductId is used in any related tables
                var isProductInUse = await _database.Table<SalesDetails>().CountAsync(s => s.ProductId == id) > 0
                                   || await _database.Table<SalesReturnDetails>().CountAsync(sr => sr.ProductId == id) > 0
                                   || await _database.Table<PurchaseDetails>().CountAsync(pm => pm.ProductId == id) > 0
                                   || await _database.Table<PurchaseReturnDetails>().CountAsync(pr => pr.ProductId == id) > 0
                                   || await _database.Table<Batch>().CountAsync(pr => pr.ProductId == id) > 0
                                   || await _database.Table<StockPosting>().CountAsync(p => p.ProductId == id) > 0;

                if (isProductInUse)
                {
                    // Product is in use, do not delete
                    return false;
                }

                // Product is not in use, proceed with deletion
                await _database.DeleteAsync<Product>(id);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error deleting Product: {ex.Message}");
                throw;
            }
        }
    }
}
