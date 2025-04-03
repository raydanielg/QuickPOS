using SQLite;
using System;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Coupon
    {
        [PrimaryKey, AutoIncrement]
        public int CouponId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(100)]  // Add a length limit to the Code field, adjust as necessary
        public string Code { get; set; }

        [MaxLength(20)]  // Limit the Type field to 20 characters (adjust if needed)
        public string Type { get; set; } = "Yes";

        public decimal Amount { get; set; }

        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; }

        public DateTime? AddedDate { get; set; }

        public DateTime? ModifyDate { get; set; }
    }
}
