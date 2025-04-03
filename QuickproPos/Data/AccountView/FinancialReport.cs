namespace QuickproPos.Data.AccountView
{
    public class FinancialReport
    {
        public int AccountGroupId { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string AccountGroupName { get; set; } // Account group for grouping
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ClosingBalance { get; set; }
        public int? ParentGroupId { get; set; } // Nullable for top-level groups

    }
}
