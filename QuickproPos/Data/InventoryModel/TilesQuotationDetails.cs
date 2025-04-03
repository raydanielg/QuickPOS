using SQLite;

namespace QuickproPos.Data.InventoryModel
{
    public class TilesQuotationDetails
    {
        [PrimaryKey, AutoIncrement]
        public int TilesQuotatinDetailsId { get; set; }
        public int TileQuotationid { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal Qty { get; set; }
        public decimal SizeLength { get; set; }
        public decimal SizeWidth { get; set; }
        public decimal TotalSqFt { get; set; }
        public string TilesSizes { get; set; }
        public decimal BoxQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
