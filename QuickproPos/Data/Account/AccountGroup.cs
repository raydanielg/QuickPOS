using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
namespace QuickproPos.Data.Account
{
    public class AccountGroup
    {
        [PrimaryKey]
        public int AccountGroupId { get; set; }

        [Required]
        [MaxLength(100)] // Optional: Add a maximum length for the name
        public string AccountGroupName { get; set; } = string.Empty;

        public int GroupUnder { get; set; }

        [Required]
        [MaxLength(20)] // Optional: Add a maximum length for the code
        public string GroupCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        [MaxLength(50)] // Optional: Limit the length of the nature description
        public string Nature { get; set; } = string.Empty;

        public DateTime? AddedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifyDate { get; set; }
    }
}
