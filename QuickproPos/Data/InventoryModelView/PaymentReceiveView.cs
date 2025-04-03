namespace QuickproPos.Data.InventoryModelView
{
    public class PaymentReceiveView
    {
        public int PaymentMasterId { get; set; }
        public int ReceiptMasterId { get; set; }
        public int ReceiptDetailsId { get; set; }
        public int PurchaseMasterId { get; set; }
        public int SalesMasterId { get; set; }
        public int PaymentDetailsId { get; set; }
        public string PurchaseVoucherNo { get; set; }
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherTypeName { get; set; }
        public string LedgerName { get; set; }
        public string Narration { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Amount { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        //GetDetailsInfo
        public decimal DueBalance { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public int LedgerId { get; set; }
        public int CashBankid { get; set; }
    }
}
