namespace QuickproPos.Data.SettingView
{
    public class CompanyView
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public int CurrencyId { get; set; }
        public int FinancialYearId { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; } = "HH:mm";
        public string DateTimeFormat => $"{DateFormat} {TimeFormat}";
        public int NoofDecimal { get; set; }
        public string Website { get; set; }
        public int WarehouseId { get; set; } //ForDefault
        public int LedgerId { get; set; } //ForDefaultCustomer
        public string TermsCondition { get; set; }
        public string Logo { get; set; }
        public string GST { get; set; }
        public string Pan { get; set; }
        public string Lut { get; set; }
        public string Iec { get; set; }
        public bool IsRound { get; set; }
        public string CurrencySymbol { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string MembershipType { get; set; }
        public string Status { get; set; }
        public string TenantId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ValidDate { get; set; }
        public bool IsPayroll { get; set; }
        public bool IsManufacture { get; set; }
        public decimal Cost { get; set; }
        public decimal Discount { get; set; }
        public int TrailDays { get; set; }
        public string IsPopular { get; set; }
        public int ProductLimit { get; set; }
        public string LicenseKey { get; set; }
        public string MachineId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int InvoiceLimit { get; set; }
        public int QuotationLimit { get; set; }
        public int CustomerLimit { get; set; }
        public int UserLimit { get; set; }
        public bool IsOnlinePayment { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string TimeZone { get; set; }
    }
}
