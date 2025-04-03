using SQLite;

namespace QuickproPos.Data.InventoryModel
{
    public class StockPosting
    {
        [PrimaryKey , AutoIncrement]
        public int StockPostingId { get; set; }
        public DateTime Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public int ProductId { get; set; }
        public int BatchId { get; set; }
        public int UnitId { get; set; }
        public int WarehouseId { get; set; }
        public int AgainstVoucherTypeId { get; set; }
        public string AgainstInvoiceNo { get; set; }
        public string AgainstVoucherNo { get; set; }
        public decimal InwardQty { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal Rate { get; set; }
        public int FinancialYearId { get; set; }
        public int DetailsId { get; set; }
        public string StockCalculate { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
