using OfficeOpenXml;
using QuickproPos.Data.Account;
using QuickproPos.Data.Setting;

namespace QuickproPos.Utilities
{
    public class ExcelImportService
    {
        public async Task<List<Product>> ReadProductsFromExcelAsync(string filePath)
        {
            var products = new List<Product>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Read the first worksheet
                var rowCount = worksheet.Dimension.Rows;

                // Start reading from the second row (skip headers)
                for (int row = 2; row <= rowCount; row++)
                {
                    var product = new Product
                    {
                        ProductCode = worksheet.Cells[row, 1]?.Text, // Column 1: Product Code
                        ProductName = worksheet.Cells[row, 2]?.Text, // Column 2: Product Name
                        HsnCode = string.Empty,
                        ProductType = "Product",
                        GroupId = int.TryParse(worksheet.Cells[row, 3]?.Text, out var groupId) ? groupId : 1,
                        BrandId = int.TryParse(worksheet.Cells[row, 4]?.Text, out var brandId) ? brandId : 1,
                        UnitId = int.TryParse(worksheet.Cells[row, 5]?.Text, out var unitId) ? unitId : 1,
                        TaxId = int.TryParse(worksheet.Cells[row, 6]?.Text, out var taxId) ? taxId : 1,
                        WarehouseId = int.TryParse(worksheet.Cells[row, 7]?.Text, out var warehouseId) ? warehouseId : 1,
                        PurchaseRate = decimal.TryParse(worksheet.Cells[row, 8]?.Text, out var purchaseRate) ? purchaseRate : 0,
                        SalesRate = decimal.TryParse(worksheet.Cells[row, 9]?.Text, out var salesRate) ? salesRate : 0,
                        Mrp = decimal.TryParse(worksheet.Cells[row, 10]?.Text, out var mrp) ? mrp : 0,
                        ApplicableOn = "Rate",
                        Narration = string.Empty,
                        QtyAlert = 0,
                        IsActive = true,
                        IsVariant = true,
                        IsBatch = true,
                        IsEcommerce = true,
                        Barcode = worksheet.Cells[row, 1]?.Text, // Column 1: Product Barcode
                        Image = string.Empty,
                        OpeningStock = 0,
                        ExiparyDate = DateTime.UtcNow,
                        AddedDate = DateTime.Now,
                    };

                    products.Add(product);
                }
            }

            return products;
        }
        public async Task<List<AccountLedger>> ReadPartiesFromExcelAsync(string filePath)
        {
            var parties = new List<AccountLedger>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Read the first worksheet
                var rowCount = worksheet.Dimension.Rows;

                // Start reading from the second row (skip headers)
                for (int row = 2; row <= rowCount; row++)
                {
                    var party = new AccountLedger
                    {
                        // Map the columns correctly based on the AccountLedger class
                        LedgerName = worksheet.Cells[row, 1]?.Text, // Column 1: Ledger Name
                        LedgerCode = worksheet.Cells[row, 2]?.Text, // Column 2: Ledger Code
                        OpeningDate = DateTime.UtcNow,
                        OpeningBalance = 0,
                        IsDefault = true,
                        CrOrDr = "Dr",
                        Address = worksheet.Cells[row, 3]?.Text, // Column 7: Address
                        Phone = worksheet.Cells[row, 4]?.Text, // Column 8: Phone
                        Email = worksheet.Cells[row, 5]?.Text, // Column 9: Email
                        ShippingAddress = string.Empty,
                        Country = string.Empty,
                        City = string.Empty,
                        TaxNo = string.Empty,
                        CreditPeriod = 0,
                        CreditLimit = 0,
                        BankName = string.Empty,
                        BranchName = string.Empty,
                        BankIFSCCode = string.Empty,
                        AccountName = string.Empty,
                        AccountNo = string.Empty,
                        AddedDate = DateTime.Now, // Use current time for AddedDate
                        ModifyDate = null, // You can update it when the record is modified
                    };

                    parties.Add(party);
                }
            }

            return parties;
        }

    }
}