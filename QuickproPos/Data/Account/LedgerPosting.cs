using SQLite;

namespace QuickproPos.Data.Account
{
    public class LedgerPosting
    {
        [PrimaryKey , AutoIncrement]
        public int LedgerPostingId { get; set; }
        public DateTime Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherNo { get; set; }
        public int LedgerId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int DetailsId { get; set; }
        public int YearId { get; set; }
        public string InvoiceNo { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public string ReferenceN { get; set; }
        public string LongReference { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
