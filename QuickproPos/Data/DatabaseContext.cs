using QuickproPos.Data.Account;
using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.Setting;
using QuickproPos.Data.User;
using QuickproPos.Data.UserView;
using QuickproPos.Utilities;
using SQLite;

namespace QuickproPos.Data
{
    public class DatabaseContext : IAsyncDisposable
    {
        private const string DbName = "QuickproPos.db3";
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbName);

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database =>
            _connection ??= new SQLiteAsyncConnection(DbPath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

        public async Task InitializeAsync()
        {
            // Define tables
            await Database.CreateTableAsync<UserMaster>();
            await Database.CreateTableAsync<AccountGroup>();
            await Database.CreateTableAsync<AccountLedger>();
            await Database.CreateTableAsync<StockPosting>();
            await Database.CreateTableAsync<LedgerPosting>();
            await Database.CreateTableAsync<PurchaseMaster>();
            await Database.CreateTableAsync<PurchaseDetails>();
            await Database.CreateTableAsync<PurchaseReturnMaster>();
            await Database.CreateTableAsync<PurchaseReturnDetails>();
            await Database.CreateTableAsync<SalesMaster>();
            await Database.CreateTableAsync<SalesDetails>();
            await Database.CreateTableAsync<SalesRegister>();
            await Database.CreateTableAsync<PosCreditNoteMaster>();
            await Database.CreateTableAsync<PosCreditNoteDetails>();
            await Database.CreateTableAsync<SalesRegisterClosingBalance>();
            await Database.CreateTableAsync<SalesReturnMaster>();
            await Database.CreateTableAsync<SalesReturnDetails>();
            await Database.CreateTableAsync<PaymentMaster>();
            await Database.CreateTableAsync<PaymentDetails>();
            await Database.CreateTableAsync<ReceiptMaster>();
            await Database.CreateTableAsync<ReceiptDetails>();
            await Database.CreateTableAsync<TilesQuotationMaster>();
            await Database.CreateTableAsync<TilesQuotationDetails>();
            await Database.CreateTableAsync<GreniteQuotation>();
            await Database.CreateTableAsync<GreniteQuotationDetails>();
            await Database.CreateTableAsync<Unit>();
            await Database.CreateTableAsync<Tax>();
            await Database.CreateTableAsync<ProductGroup>();
            await Database.CreateTableAsync<Brand>();
            await Database.CreateTableAsync<Batch>();
            await Database.CreateTableAsync<Company>();
            await Database.CreateTableAsync<Coupon>();
            await Database.CreateTableAsync<EmailSetting>();
            await Database.CreateTableAsync<FinancialYear>();
            await Database.CreateTableAsync<GeneralSetting>();
            await Database.CreateTableAsync<InvoiceSetting>();
            await Database.CreateTableAsync<PaymentType>();
            await Database.CreateTableAsync<Currency>();
            await Database.CreateTableAsync<Privilege>();
            await Database.CreateTableAsync<Warehouse>();
            await Database.CreateTableAsync<Product>();
        }

        public SQLiteAsyncConnection GetConnection() => Database;
        
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection = null;
            }
        }
        //SeedData for the account group table
        public async Task SeedAccountgroupDataAsync()
        {
            var accountgroup = await Database.Table<AccountGroup>().FirstOrDefaultAsync();
            if (accountgroup == null)
            {
                AccountGroupInfo info = new AccountGroupInfo();
                List<AccountGroup> listGroup = new List<AccountGroup>();
                listGroup = info.GetAccountGroupInfo();
                // Insert default account group data if the table is empty
                foreach (var item in listGroup)
                {
                    await Database.InsertAsync(item);
                }
            }
        }
        //SeedData for the account ledger table
        public async Task SeedAccountledgerDataAsync()
        {
            var accountledger = await Database.Table<AccountLedger>().FirstOrDefaultAsync();
            if (accountledger == null)
            {
                AccountLedgerInfo info = new AccountLedgerInfo();
                List<AccountLedger> listLedger = new List<AccountLedger>();
                listLedger = info.GetAccountLedgerInfo();
                // Insert default account group data if the table is empty
                foreach (var item in listLedger)
                {
                    await Database.InsertAsync(item);
                }
            }
        }
        public async Task SeedTaxDataAsync()
        {
            var tax = await Database.Table<Tax>().FirstOrDefaultAsync();
            if (tax == null)
            {
                // Insert default Tax data if the table is empty
                var defaultTax = new Tax
                {
                    TaxName = "NA",
                    Rate = 0,
                    IsActive = true,
                    AddedDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultTax);
            }
        }
        //SeedData for the unit table
        public async Task SeedUnitDataAsync()
        {
            var unit = await Database.Table<Unit>().FirstOrDefaultAsync();
            if (unit == null)
            {
                UnitInfo info = new UnitInfo();
                List<Unit> listUnit = new List<Unit>();
                listUnit = info.GetUnitInfo();
                // Insert default unit data if the table is empty
                foreach (var item in listUnit)
                {
                    await Database.InsertAsync(item);
                }
            }
        }

        //SeedData for the PaymentType table
        public async Task SeedPaymentTypeDataAsync()
        {
            var paymentType = await Database.Table<PaymentType>().FirstOrDefaultAsync();
            if (paymentType == null)
            {
                PaymentTypeInfo info = new PaymentTypeInfo();
                List<PaymentType> listPaymentType = new List<PaymentType>();
                listPaymentType = info.GetPaymentTypeInfo();
                // Insert default listPaymentType data if the table is empty
                foreach (var item in listPaymentType)
                {
                    await Database.InsertAsync(item);
                }
            }
        }
        // Seed data for the user table
        public async Task SeedUserDataAsync()
        {
            var user = await Database.Table<UserMaster>().FirstOrDefaultAsync();
            if (user == null)
            {
                // Insert default user data if the table is empty
                var defaultuser = new UserMaster
                {
                    Username = "Bryan Hamal",
                    Email = "admin@gmail.com",
                    Password = "admin123",
                    IsEmailVerified = true,
                    IsActive = true,
                    Role = UserRole.Admin.ToString(),
                    Image = string.Empty,
                    AddedDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultuser);
            }
        }
        // Seed data for the Company table
        public async Task SeedCompanyDataAsync()
        {
            var company = await Database.Table<Company>().FirstOrDefaultAsync();
            if (company == null)
            {
                // Insert default company data if the table is empty
                var defaultCompany = new Company
                {
                    CompanyName = "QuickBooks Pro",
                    Address = "123 Business St, City, Country",
                    MobileNo = "+1234567890",
                    Email = "info@QuickproPos.com",
                    CurrencyId = 1, // Assuming the first currency exists
                    FinancialYearId = 1, // Assuming a financial year exists
                    NoofDecimal = 2,
                    Website = "https://QuickproPos.com",
                    WarehouseId = 1, // Assuming a warehouse exists
                    LedgerId = 1, // Assuming a ledger exists
                    GST = "GST123456789",
                    Pan = "PAN123456789",
                    Lut = "LUT123456789",
                    Iec = "IEC123456789",
                    Logo = "https://QuickproPos.com/logo.png",
                    LicenseKey = string.Empty,
                    MachineId = string.Empty,
                    Date = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsDefault = true,
                    StartDate = DateTime.UtcNow,
                    ValidDate = DateTime.UtcNow.AddYears(1),
                    AddedDate = DateTime.UtcNow,
                    ModifyDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultCompany);
            }
        }

        // Seed data for the InvoiceSetting table
        public async Task SeedInvoiceSettingDataAsync()
        {
            var invoiceSetting = await Database.Table<InvoiceSetting>().FirstOrDefaultAsync();
            if (invoiceSetting == null)
            {
                InvoiceSettingInfo infoInvoice = new InvoiceSettingInfo();
                List<InvoiceSetting> listInvoice = new List<InvoiceSetting>();
                listInvoice = infoInvoice.GetInvoiceSettingInfo();
                // Insert default invoice setting data if the table is empty
                foreach (var item in listInvoice)
                {
                    await Database.InsertAsync(item);
                }
            }
        }
        public async Task SeedCategoryDataAsync()
        {
            var category = await Database.Table<ProductGroup>().FirstOrDefaultAsync();
            if (category == null)
            {
                // Insert default category data if the table is empty
                var defaultcategory = new ProductGroup
                {
                    GroupName = "GENERAL",
                    GroupUnder = 0,
                    Image = string.Empty,
                    Narration = string.Empty,
                    AddedDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultcategory);
            }
        }

        public async Task SeedWarehouseDataAsync()
        {
            var warehouse = await Database.Table<Warehouse>().FirstOrDefaultAsync();
            if (warehouse == null)
            {
                // Insert default warehouse data if the table is empty
                var defaultwarehouse = new Warehouse
                {
                    Name = "Main Warehouse",
                    Email = string.Empty,
                    Country = string.Empty,
                    City = string.Empty,
                    Mobile = string.Empty,
                    AddedDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultwarehouse);
            }
        }
        public async Task SeedCurrencyDataAsync()
        {
            var currency = await Database.Table<Currency>().FirstOrDefaultAsync();
            if (currency == null)
            {
                // Insert default currency data if the table is empty
                var defaultCurrency = new Currency
                {
                    CurrencyName = "USD",
                    CurrencySymbol = "$",
                    NoOfDecimalPlaces = 2,
                    IsDefault = true,
                    AddedDate = DateTime.UtcNow
                };
                await Database.InsertAsync(defaultCurrency);
            }
        }
    }
}
