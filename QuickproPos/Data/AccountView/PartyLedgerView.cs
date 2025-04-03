namespace QuickproPos.Data.AccountView
{
    public class PartyLedgerView
    {
        public string LedgerName { get; set; }  // Name of the Party (or Ledger)
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
