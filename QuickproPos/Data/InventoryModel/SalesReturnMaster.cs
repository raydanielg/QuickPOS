using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickproPos.Data.InventoryModel
{
    public class SalesReturnMaster
    {
        [PrimaryKey , AutoIncrement]
        public int SalesReturnMasterId { get; set; }
        public string SerialNo { get; set; }
        public string VoucherNo { get; set; }
        public int WarehouseId { get; set; }
        public int VoucherTypeId { get; set; }
        public DateTime Date { get; set; }
        public int LedgerId { get; set; }
        public int SalesMasterId { get; set; }
        public string Narration { get; set; }
        public int TaxId { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DisPer { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmounts { get; set; }
        public decimal PayAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal RoundOff { get; set; }
        public string Status { get; set; }
        public decimal PreviousDue { get; set; }
        public int PaymentId { get; set; }
        public string UserId { get; set; }
        public int FinancialYearId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [Ignore]  // Ignore this property for table creation
        public List<SalesReturnDetails> listOrder { get; set; } = new List<SalesReturnDetails>();
        [Ignore]  // Ignore this property for table creation
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
    }
}
