namespace QuickproPos.Data.InventoryModelView
{
    public class SalesRegisterClosingBalanceView
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosingTime { get; set; }
        public decimal OpeningCashAmount { get; set; }
        public decimal ClosingCashAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal PayLater { get; set; }
        public decimal CreditAppliedAmount { get; set; }
        public decimal SalesReturnAmount { get; set; }
        public decimal ClosingAmount { get; set; }
        public decimal CashReturnAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public decimal PurchasePayment { get; set; }
        public decimal Cash { get; set; }
        public decimal Card { get; set; }
        public decimal UPI { get; set; }
        public decimal TotalSales { get; set; }
        public int PaymentId { get; set; }
        public string FullName { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
    }
}
