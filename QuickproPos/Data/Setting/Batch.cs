using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
namespace QuickproPos.Data.Setting
{
    public class Batch
    {
        [PrimaryKey, AutoIncrement]
        public int BatchId { get; set; }

        [Required]  // Optionally add validation attributes
        [MaxLength(50)]  // Optional length limit
        public string BatchNo { get; set; }

        public int ProductId { get; set; }

        public DateTime MfgDate { get; set; }

        public DateTime ExpDate { get; set; }

        public decimal SalesRate { get; set; }

        public decimal PurchaseRate { get; set; }

        public decimal Mrp { get; set; }

        public bool Status { get; set; }
    }
}
