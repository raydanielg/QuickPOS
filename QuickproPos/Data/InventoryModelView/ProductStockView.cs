namespace QuickproPos.Data.InventoryModelView
{
    public class ProductStockView
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal? Rate { get; set; }
        public decimal PurQty { get; set; }
        public decimal? PurchaseStockBal { get; set; }
        public decimal SalesRate { get; set; }
        public decimal SalesQty { get; set; }
        public decimal? SalesStockBalance { get; set; }
        public decimal Stock { get; set; }
        public decimal? StockValue { get; set; }
    }
}
