using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class ProductGroup
    {
        [PrimaryKey, AutoIncrement]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(255)] // Enforcing a maximum length for GroupName
        public string GroupName { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid group.")]
        public int GroupUnder { get; set; }

        public string Image { get; set; }

        [MaxLength(1000)] // Limiting the narration text size
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
