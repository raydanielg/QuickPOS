using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class GeneralSetting
    {
        [PrimaryKey, AutoIncrement]
        public int GeneralId { get; set; }

        // Using strings to store boolean-like values
        [MaxLength(3)] // Limiting to values like "Yes", "No", "Warn"
        public string ShowCurrency { get; set; } = "Yes";

        [MaxLength(4)] // Limiting to values like "Warn"
        public string NegativeCash { get; set; } = "Warn";

        [MaxLength(4)] // Limiting to values like "Warn"
        public string NegativeStock { get; set; } = "Warn";

        [MaxLength(3)] // Limiting to values like "FIFO"
        public string StockCalculationMode { get; set; } = "FIFO";

        [MaxLength(4)] // Limiting to values like "Warn"
        public string CreditLimit { get; set; } = "Warn";

        [MaxLength(12)] // Limiting to values like "No Discount"
        public string Discount { get; set; } = "No Discount";

        [MaxLength(3)] // Limiting to values like "Yes"
        public string VatOnPurchase { get; set; } = "Yes";

        [MaxLength(3)] // Limiting to values like "Yes"
        public string VatOnSales { get; set; } = "Yes";

        [MaxLength(3)] // Limiting to values like "Yes"
        public string ShowBatch { get; set; } = "Yes";

        [MaxLength(4)] // Limiting to values like "Show"
        public string BatchDate { get; set; } = "Show";
    }
}
