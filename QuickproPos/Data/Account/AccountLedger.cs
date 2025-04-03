using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Account
{
    public class AccountLedger
    {
        [PrimaryKey]
        public int LedgerId { get; set; }

        [Required]
        public int AccountGroupId { get; set; } // Consider adding foreign key constraints in the database

        [Required]
        [MaxLength(100)] // Optional: Enforce a maximum length for the ledger name
        public string LedgerName { get; set; } = string.Empty;

        [MaxLength(50)] // Optional: Enforce a maximum length for the ledger code
        public string LedgerCode { get; set; } = string.Empty;

        public DateTime OpeningDate { get; set; } = DateTime.UtcNow;

        public decimal OpeningBalance { get; set; } = 0;

        public bool IsDefault { get; set; } = false;

        [MaxLength(3)] // Optional: Use a maximum length for "Cr" or "Dr"
        public string CrOrDr { get; set; } = string.Empty;

        [MaxLength(200)] // Optional: Enforce maximum length for address fields
        public string Address { get; set; } = string.Empty;

        [MaxLength(15)] // Optional: Restrict phone number length
        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(50)]
        public string City { get; set; } = string.Empty;
        [MaxLength(50)]
        public string TaxNo { get; set; } = string.Empty;
        public int CreditPeriod { get; set; } = 0; // Number of days, ensure defaults are meaningful
        public decimal CreditLimit { get; set; } = 0;
        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string BranchName { get; set; } = string.Empty;
        [MaxLength(20)]
        public string BankIFSCCode { get; set; } = string.Empty;
        [MaxLength(100)]
        public string AccountName { get; set; } = string.Empty;
        [MaxLength(20)]
        public string AccountNo { get; set; } = string.Empty;
        public DateTime? AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifyDate { get; set; }
    }
}
