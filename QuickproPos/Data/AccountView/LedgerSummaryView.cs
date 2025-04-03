namespace QuickproPos.Data.AccountView
{
    public class LedgerSummaryView
    {
        public int SlNo { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string Opening { get; set; }
        public decimal Op { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Closing { get; set; }
        public string Closing1 { get; set; }
    }

}
