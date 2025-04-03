using QuickproPos.Data.Account;

namespace QuickproPos.Utilities
{
    public class AccountLedgerInfo
    {
        public List<AccountLedger> GetAccountLedgerInfo()
        {
            var info = new List<AccountLedger>();
            info.Add(new AccountLedger()
            {
                LedgerId = 1,
                LedgerName = "Cash", // Example values for demonstration.
                LedgerCode = "001",
                OpeningBalance = 0,
                AccountGroupId = 27,
                OpeningDate = DateTime.Now,
                CrOrDr = "Dr",
                Address = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                ShippingAddress = string.Empty,
                Country = string.Empty,
                City = string.Empty,
                TaxNo = string.Empty,
                CreditPeriod = 0,
                CreditLimit = 0,
                BankName = string.Empty,
                BranchName = string.Empty,
                BankIFSCCode = string.Empty,
                AccountName = string.Empty,
                AccountNo = string.Empty,
                IsDefault = true,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountLedger()
            {
                LedgerId = 2,
                LedgerName = "Dutiex & Taxes", // Example values for demonstration.
                LedgerCode = "002",
                OpeningBalance = 0,
                AccountGroupId = 20,
                OpeningDate = DateTime.Now,
                CrOrDr = "Dr",
                Address = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                ShippingAddress = string.Empty,
                Country = string.Empty,
                City = string.Empty,
                TaxNo = string.Empty,
                CreditPeriod = 0,
                CreditLimit = 0,
                BankName = string.Empty,
                BranchName = string.Empty,
                BankIFSCCode = string.Empty,
                AccountName = string.Empty,
                AccountNo = string.Empty,
                IsDefault = true,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountLedger()
            {
                LedgerId = 3,
                LedgerName = "Sales Account", // Example values for demonstration.
                LedgerCode = "003",
                OpeningBalance = 0,
                AccountGroupId = 10,
                OpeningDate = DateTime.Now,
                CrOrDr = "Dr",
                Address = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                ShippingAddress = string.Empty,
                Country = string.Empty,
                City = string.Empty,
                TaxNo = string.Empty,
                CreditPeriod = 0,
                CreditLimit = 0,
                BankName = string.Empty,
                BranchName = string.Empty,
                BankIFSCCode = string.Empty,
                AccountName = string.Empty,
                AccountNo = string.Empty,
                IsDefault = true,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountLedger()
            {
                LedgerId = 4,
                LedgerName = "Purchase Account", // Example values for demonstration.
                LedgerCode = "004",
                OpeningBalance = 0,
                AccountGroupId = 11,
                OpeningDate = DateTime.Now,
                CrOrDr = "Dr",
                Address = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                ShippingAddress = string.Empty,
                Country = string.Empty,
                City = string.Empty,
                TaxNo = string.Empty,
                CreditPeriod = 0,
                CreditLimit = 0,
                BankName = string.Empty,
                BranchName = string.Empty,
                BankIFSCCode = string.Empty,
                AccountName = string.Empty,
                AccountNo = string.Empty,
                IsDefault = true,
                AddedDate = DateTime.UtcNow
            });
            return info;
        }
    }
}
