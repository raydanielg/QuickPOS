using QuickproPos.Data;

namespace QuickproPos
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseContext _databaseContext;

        public MainPage()
        {
            InitializeComponent();
            // Initialize DatabaseContext
            _databaseContext = new DatabaseContext();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Call seed methods when the page appears
            await SeedDataAsync();
        }
        private async Task SeedDataAsync()
        {
            await _databaseContext.InitializeAsync();
            await _databaseContext.SeedCompanyDataAsync();
            await _databaseContext.SeedUserDataAsync();
            await _databaseContext.SeedAccountgroupDataAsync();
            await _databaseContext.SeedAccountledgerDataAsync();
            await _databaseContext.SeedTaxDataAsync();
            await _databaseContext.SeedUnitDataAsync();
            await _databaseContext.SeedPaymentTypeDataAsync();
            await _databaseContext.SeedCategoryDataAsync();
            await _databaseContext.SeedWarehouseDataAsync();
            await _databaseContext.SeedInvoiceSettingDataAsync();
            await _databaseContext.SeedCurrencyDataAsync();
        }
    }
}