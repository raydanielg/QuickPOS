using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Unit
    {
        [PrimaryKey, AutoIncrement]
        public int UnitId { get; set; }

        [Required]
        [MaxLength(255)] // Adds a reasonable length constraint
        public string UnitName { get; set; }

        [Range(0, 10, ErrorMessage = "Number of decimal places must be between 0 and 10.")]
        public int NoOfDecimalplaces { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
