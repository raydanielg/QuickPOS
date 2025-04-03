using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickproPos.Data.InventoryModel
{
    public class SalesMaster
    {
        [PrimaryKey , AutoIncrement]
        public int SalesMasterId { get; set; }
        [Required]
        public string SerialNo { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        public int WarehouseId { get; set; }
        public int VoucherTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int LedgerId { get; set; }
        public int OrderMasterId { get; set; }
        public int QuotationMasterId { get; set; }
        public int DeliveryNoteMasterId { get; set; }
        public string Narration { get; set; }
        public int TaxId { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TaxRate { get; set; }
        public string Reference { get; set; }
        public string PaymentStatus { get; set; }
        public decimal DisPer { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmounts { get; set; }
        public decimal PayAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal RoundOff { get; set; }
        public string Status { get; set; }
        public decimal PreviousDue { get; set; }
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Channel { get; set; }
        public int FinancialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [Ignore]  // Ignore this property for table creation
        public List<SalesDetails> listOrder { get; set; } = new List<SalesDetails>();
        [Ignore]  // Ignore this property for table creation
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
        [Ignore]  // Ignore this property for table creation
        public bool IsPayment { get; set; }
        [Ignore]  // Ignore this property for table creation
        public int CashBankId { get; set; }
        [Ignore]  // Ignore this property for table creation
        public string ReferenceReceipt { get; set; }
        [Ignore]  // Ignore this property for table creation
        public decimal TenderAmount { get; set; }
        [Ignore]  // Ignore this property for table creation
        public decimal CashReturn { get; set; }
    }
}
