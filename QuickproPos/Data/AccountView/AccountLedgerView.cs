using System.ComponentModel.DataAnnotations;

namespace QuickproPos.Data.AccountView
{
    public class AccountLedgerView
    {
        public int LedgerId { get; set; }
        public int AccountGroupId { get; set; }

        public string AccountGroupName { get; set; } // Consider adding foreign key constraints in the database

        public string LedgerName { get; set; } = string.Empty;

        public string LedgerCode { get; set; } = string.Empty;

        public DateTime OpeningDate { get; set; } = DateTime.UtcNow;

        public decimal OpeningBalance { get; set; } = 0;

        public bool IsDefault { get; set; } = false;

        public string CrOrDr { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string TaxNo { get; set; } = string.Empty;
        public int CreditPeriod { get; set; } = 0; // Number of days, ensure defaults are meaningful
        public decimal CreditLimit { get; set; } = 0;
        public string BankName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BankIFSCCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public DateTime? AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifyDate { get; set; }
    }
}
