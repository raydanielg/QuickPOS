using SQLite;

namespace QuickproPos.Data.InventoryModel
{
    public class SalesRegister
    {
        [PrimaryKey, AutoIncrement]
        public int SalesRegisterId { get; set; }
        public int UserId { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosingTime { get; set; }
        public decimal OpeningCashAmount { get; set; }
        public decimal ClosingCashAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public decimal CashLeftinDrawer { get; set; }
        public decimal PhysicalDrawer { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
