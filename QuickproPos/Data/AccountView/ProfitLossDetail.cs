namespace QuickproPos.Data.AccountView
{
    public class ProfitLossDetail
    {
        public string LedgerName { get; set; }  // Ledger name
        public decimal Debit { get; set; }      // Total Debit
        public decimal Credit { get; set; }     // Total Credit
        public decimal ClosingBalance { get; set; } // Closing balance after calculating Debit - Credit
    }

}
