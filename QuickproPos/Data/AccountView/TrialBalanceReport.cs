namespace QuickproPos.Data.AccountView
{
    public class TrialBalanceReport
    {
        public int AccountGroupId { get; set; }
        public string AccountGroupName { get; set; }
        public List<LedgerReport> Ledgers { get; set; } = new List<LedgerReport>();
    }

    public class LedgerReport
    {
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
