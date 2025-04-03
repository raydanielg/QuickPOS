using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class FinancialYear
    {
        [PrimaryKey, AutoIncrement]
        public int FinancialYearId { get; set; }

        [Required(ErrorMessage = "Type from date")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Type to date")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Type fiscal year")]
        [MaxLength(10)]  // Adjust length based on your fiscal year format
        public string FiscalYear { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
