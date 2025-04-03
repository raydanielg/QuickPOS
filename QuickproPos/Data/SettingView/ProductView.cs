namespace QuickproPos.Data.SettingView
{
    public class ProductView
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public string ProductName { get; set; }
        public int UnitId { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal SalesRate { get; set; }
        public decimal Mrp { get; set; }
        public string Narration { get; set; }
        public bool IsActive { get; set; }
        public string Barcode { get; set; }
        public string Image { get; set; }
        public string UnitName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public decimal FlatDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int TaxId { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ShippingAmount { get; set; }
        public string Includingpurchasetax { get; set; }
        public string Includingsalestax { get; set; }
        public decimal TaxRate { get; set; }
        public string TaxName { get; set; }
        public string BrandName { get; set; }
        public bool IsBatch { get; set; }
        public string BatchNo { get; set; }
        public int BatchId { get; set; }
        public string ApplicableOn { get; set; }
        public int CurrentStock { get; set; }
        public bool Isshow { get; set; }
        public decimal Stock { get; set; }
        public string Type { get; set; }
        public string HsnCode { get; set; }
        public DateTime Date { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? MfgDate { get; set; }
        public DateTime? ExpDate { get; set; }
        //PurchaseInvoice
        public int PurchaseDetailsId { get; set; }
        public int PurchaseOrderDetailsId { get; set; }
        public int ReceiptDetailsId { get; set; }
        public int OrderDetailsId { get; set; }
        //PurchaseReturn
        public int PurchaseReturnDetailsId { get; set; }

        //SalesInvoice
        public int SalesDetailsId { get; set; }
        public int DeliveryNoteDetailsId { get; set; }
        public int QuotationDetailsId { get; set; }
        public int SalesOrderDetailsId { get; set; }

        //SalesReturn
        public int SalesReturnMasterId { get; set; }
        public int SalesReturnDetailsId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        //StockPosting
        public int StockPostingId { get; set; }
        public int StockAdjustmentDetailsId { get; set; }
        public int StockTransferDetailsId { get; set; }
        public int BomDetailsId { get; set; }
        public int AssembledItemDetailsId { get; set; }



        //TilesQuotation
        public int TilesQuotatinDetailsId { get; set; }
        public int TileQuotationid { get; set; }
        public decimal SizeLength { get; set; }
        public decimal SizeWidth { get; set; }
        public string TilesSizes { get; set; }
        public decimal BoxQty { get; set; }
        public string Description { get; set; }

        //GreniteQuotation
        public int GreniteQuotationDetailsId { get; set; }
        public decimal MeasurementSrNo { get; set; }
        public decimal TotalSqFt { get; set; }
    }
}