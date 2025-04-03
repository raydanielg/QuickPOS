namespace QuickproPos.Data.InventoryModelView
{
    public class SalesMasterView
    {
        public int SalesMasterId { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public string NepaliDate { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherTypeName { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string Narration { get; set; }
        public decimal NetAmounts { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDue { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TotalTax { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Pan { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal TaxRate { get; set; }
        public string Status { get; set; }
        public string WarehouseName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Reference { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
