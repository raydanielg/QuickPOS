using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Brand
    {
        [PrimaryKey, AutoIncrement]
        public int BrandId { get; set; }

        // Ensure Name is required and give a custom error message
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(100)]  // Optionally specify max length for Name
        public string Name { get; set; }

        public string Image { get; set; }

        public DateTime? AddedDate { get; set; }

        public DateTime? ModifyDate { get; set; }
    }
}
