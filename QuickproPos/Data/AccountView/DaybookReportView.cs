namespace QuickproPos.Data.AccountView
{
    public class DaybookReportView
    {
        public string Date { get; set; }
        public string VoucherTypeName { get; set; }
        public string VoucherNo { get; set; }
        public string LedgerName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Narration { get; set; }
    }
}
