using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Company
    {
        [PrimaryKey , AutoIncrement]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(200)]  // Limit the length of the CompanyName
        public string CompanyName { get; set; }

        [Required]
        [MaxLength(500)]  // Limit the length of Address
        public string Address { get; set; }

        [MaxLength(15)]  // Restrict mobile number length
        public string MobileNo { get; set; }

        [Required]
        [EmailAddress]  // Ensures the Email is a valid email format
        [MaxLength(255)]  // Limit the length of Email
        public string Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a currency.")]
        public int CurrencyId { get; set; }

        public int FinancialYearId { get; set; }
        public int NoofDecimal { get; set; }

        [MaxLength(200)]  // Limit website URL length
        public string Website { get; set; }

        public int WarehouseId { get; set; }
        public int LedgerId { get; set; }

        [MaxLength(50)]  // Limit GST number length
        public string GST { get; set; }

        [MaxLength(20)]  // Limit PAN number length
        public string Pan { get; set; }

        [MaxLength(50)]  // Limit LUT number length
        public string Lut { get; set; }

        [MaxLength(20)]  // Limit IEC number length
        public string Iec { get; set; }
        public string Logo { get; set; }
        public string LicenseKey { get; set; }
        public string MachineId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpiryDate { get; set; }

        public bool IsDefault { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ValidDate { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
