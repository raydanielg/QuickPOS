using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class InvoiceSetting
    {
        [PrimaryKey]
        public int VoucherTypeId { get; set; }

        [Required]
        [MaxLength(100)] // Limit the length of the VoucherTypeName to 100 characters
        public string VoucherTypeName { get; set; }

        [MaxLength(50)] // Limiting string lengths for consistency
        public string Suffix { get; set; }

        [MaxLength(50)] // Limiting string lengths for consistency
        public string Prefix { get; set; }

        public int StartIndex { get; set; }
    }
}
