using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class PaymentType
    {
        [PrimaryKey]
        public int PaymentTypeId { get; set; }

        [Required]
        [MaxLength(255)] // SQLite does not enforce string length, but good practice
        public string Name { get; set; }
    }
}
