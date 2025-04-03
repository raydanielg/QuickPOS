using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Tax
    {
        [PrimaryKey, AutoIncrement]
        public int TaxId { get; set; }

        [Required]
        [MaxLength(255)] // Enforces a reasonable length for the name
        public string TaxName { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
