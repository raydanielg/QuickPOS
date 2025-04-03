using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)] // Limit the size for better performance
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [MaxLength(50)]
        public string HsnCode { get; set; }

        [MaxLength(50)]
        public string ProductType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int GroupId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a brand.")]
        public int BrandId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a Unit.")]
        public int UnitId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a Tax.")]
        public int TaxId { get; set; }

        public int WarehouseId { get; set; }

        public decimal PurchaseRate { get; set; }

        public decimal SalesRate { get; set; }

        public decimal Mrp { get; set; }

        [MaxLength(50)]
        public string ApplicableOn { get; set; }

        public string Narration { get; set; }

        public int QtyAlert { get; set; }
        public string Includingpurchasetax { get; set; }
        public string Includingsalestax { get; set; }
        public bool IsActive { get; set; }
        public bool IsVariant { get; set; }
        public bool IsBatch { get; set; }
        public bool IsEcommerce { get; set; }

        [MaxLength(100)]
        public string Barcode { get; set; }

        public string Image { get; set; }

        public int OpeningStock { get; set; }

        public DateTime ExiparyDate { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
