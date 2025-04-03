namespace QuickproPos.Data.AccountView
{
    public class DashboardView
    {
        public int TotalCustomer { get; set; }
        public int TotalSupplier { get; set; }
        public string Month { get; set; }
        public decimal InwardQty { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal SalesReturn { get; set; }
        public decimal PurchaseReturn { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Purchase { get; set; }
        public decimal Receive { get; set; }
        public decimal Payment { get; set; }
        public decimal GrandTotal { get; set; }
        public string SalesBill { get; set; }
        public string PurchaseBill { get; set; }
        public decimal CashBalance { get; set; }
        public decimal BankBalance { get; set; }
        public int TotalProducts { get; set; }
    }
}
