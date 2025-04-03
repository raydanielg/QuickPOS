using SQLite;

namespace QuickproPos.Data.InventoryModel
{
    public class GreniteQuotation
    {
        [PrimaryKey , AutoIncrement]
        public int GreniteQuotationId { get; set; }
        public int FinancialYearId { get; set; }
        public int VoucherTypeId { get; set; }
        public int LedgerId { get; set; }
        public string SerialNo { get; set; }
        public string QuotationNo { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalSqft { get; set; } = 0;
        public decimal TotalQty { get; set; } = 0;
        public decimal TotalTax { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public decimal AdvanceAmount { get; set; } = 0;
        public decimal PendingAmount { get; set; }
        public decimal GrandTotal { get; set; } = 0;
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [Ignore]  // Ignore this property for table creation
        public List<GreniteQuotationDetails> listOrder { get; set; } = new List<GreniteQuotationDetails>();

        [Ignore]  // Ignore this property for table creation
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
    }
}
