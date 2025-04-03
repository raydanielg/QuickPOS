using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Currency
    {
        [PrimaryKey, AutoIncrement]
        public int CurrencyId { get; set; }
        [Required]
        [MaxLength(20)]
        public string CurrencySymbol { get; set; }
        [Required]
        [MaxLength(40)]
        public string CurrencyName { get; set; }
        public int NoOfDecimalPlaces { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
