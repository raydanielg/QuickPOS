using SQLite;

namespace QuickproPos.Data.InventoryModel
{
    public class GreniteQuotationDetails
    {
        [PrimaryKey , AutoIncrement]
        public int GreniteQuotationDetailsId { get; set; }
        public int GreniteQuotationId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal Qty { get; set; }
        public decimal MeasurementSrNo { get; set; }
        public decimal SizeLength { get; set; }
        public decimal SizeWidth { get; set; }
        public decimal TotalSqFt { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
