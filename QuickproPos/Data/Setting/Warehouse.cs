using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Warehouse
    {
        [PrimaryKey, AutoIncrement]
        public int WarehouseId { get; set; }

        [Required]
        [MaxLength(255)] // Adds a reasonable limit for the name
        public string Name { get; set; }

        [MaxLength(15)] // Typical mobile number length limit
        public string Mobile { get; set; }

        [MaxLength(100)] // Adds a length constraint for country
        public string Country { get; set; }

        [MaxLength(100)] // Adds a length constraint for city
        public string City { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(255)] // Ensures email length is within reasonable limits
        public string Email { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
