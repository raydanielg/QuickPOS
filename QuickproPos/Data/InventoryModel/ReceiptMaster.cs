using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickproPos.Data.InventoryModel
{
    public class ReceiptMaster
    {
        [PrimaryKey , AutoIncrement]
        public int ReceiptMasterId { get; set; }
        public int SalesMasterId { get; set; }
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public int LedgerId { get; set; }
        public decimal PaidAmount { get; set; }
        public Decimal Amount { get; set; }
        public string SerialNo { get; set; }
        public string Narration { get; set; }
        public int VoucherTypeId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int FinancialYearId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [Ignore]  // Ignore this property for table creation
        public List<ReceiptDetails> listOrder { get; set; } = new List<ReceiptDetails>();
        [Ignore]  // Ignore this property for table creation
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
        [Ignore]  // Ignore this property for table creation
        public decimal PreviousDue { get; set; }
    }
}
