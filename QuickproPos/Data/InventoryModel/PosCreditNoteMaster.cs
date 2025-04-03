using SQLite;
using System.ComponentModel.DataAnnotations;

namespace QuickproPos.Data.InventoryModel
{
    public class PosCreditNoteMaster
    {
        [PrimaryKey , AutoIncrement]
        public int CreditNoteId { get; set; }
        public int SalesMasterId { get; set; }
        [Required]
        public string SerialNo { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select Warehouse")]
        public int WarehouseId { get; set; }
        public int VoucherTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int LedgerId { get; set; }
        public string Narration { get; set; }
        public int TaxId { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TaxRate { get; set; }
        public string Reference { get; set; }
        public decimal DisPer { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PayAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal RoundOff { get; set; }
        public decimal PreviousDue { get; set; }
        public int PaymentId { get; set; }
        public string UserId { get; set; }
        public bool POS { get; set; }
        public string Status { get; set; }
        public decimal CreditsUsed { get; set; }
        public decimal CreditRemaining { get; set; }
        public int FinancialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [Ignore]
        public List<PosCreditNoteDetails> listOrder { get; set; } = new List<PosCreditNoteDetails>();
        [Ignore]
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
        [Ignore]
        public decimal ApplyCredit { get; set; }
        [Ignore]
        public string CashReturnType { get; set; }
    }
}
