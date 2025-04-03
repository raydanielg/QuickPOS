using System.ComponentModel.DataAnnotations;

namespace QuickproPos.Data
{
    public class DeleteItem
    {
        [Key]
        public int id { get; set; }
        public int PurchaseDetailsId { get; set; }
        public int DeleteJournalDetailsId { get; set; }
        public int SalesDetailsId { get; set; }
        public int PurchaseReturnDetailsId { get; set; }
        public int SalesReturnDetailsId { get; set; }
        public int QuotationDetailsId { get; set; }
        public int PaymentDetailsId { get; set; }
        public int ReceiptDetailsId { get; set; }
        public int PurchaseOrderDetailsId { get; set; }
        public int SalesOrderDetailsId { get; set; }
        public int TilesQuotatinDetailsId { get; set; }
        public int GreniteQuotationDetailsId { get; set; }
    }
}
