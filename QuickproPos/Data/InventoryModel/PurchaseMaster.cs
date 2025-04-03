using SQLite;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace QuickproPos.Data.InventoryModel
{
    public class PurchaseMaster
    {
        [PrimaryKey, AutoIncrement]
        public int PurchaseMasterId { get; set; }

        [Required]
        public string SerialNo { get; set; }

        [Required]
        public string VoucherNo { get; set; }

        public string VendorInvoiceNo { get; set; }

        public DateTime VendorInvoiceDate { get; set; }

        public int WarehouseId { get; set; }

        public int VoucherTypeId { get; set; }

        // Date as a DateTime, stored as string if necessary using custom logic
        [Required]
        public DateTime Date { get; set; }

        public DateTime DueDate { get; set; }

        public int LedgerId { get; set; }

        public string Reference { get; set; }

        public string Narration { get; set; }

        public int PurchaseOrderMasterId { get; set; }

        public int MaterialReceiptMasterId { get; set; }

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

        public string PaymentStatus { get; set; }

        public decimal PreviousDue { get; set; }

        public string UserId { get; set; }

        public int FinancialYearId { get; set; }

        public DateTime? AddedDate { get; set; }

        public DateTime? ModifyDate { get; set; }

        [Ignore]  // Ignore this property for table creation
        public List<PurchaseDetails> listOrder { get; set; } = new List<PurchaseDetails>();

        [Ignore]  // Ignore this property for table creation
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();

        // Optional: If you want to store `Date` as a string in SQLite (for custom formatting), you can use:
        [Ignore]
        public string DateString
        {
            get => Date.ToString("yyyy-MM-dd HH:mm:ss");
            set => Date = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
