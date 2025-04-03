using QuickproPos.Data.Account;

namespace QuickproPos.Utilities
{
    public class AccountGroupInfo
    {
        public List<AccountGroup> GetAccountGroupInfo()
        {
            var info = new List<AccountGroup>();
            info.Add(new AccountGroup()
            {
                AccountGroupId = 0,
                AccountGroupName = "Primary",
                GroupUnder = -1,
                GroupCode = "0",
                IsDefault = true,
                Nature = "NA",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 1,
                AccountGroupName = "Capital Account",
                GroupUnder = 0,
                GroupCode = "1",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 2,
                AccountGroupName = "Loans (Liability)",
                GroupUnder = 0,
                GroupCode = "2",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 3,
                AccountGroupName = "Current Liabilities",
                GroupUnder = 0,
                GroupCode = "3",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 4,
                AccountGroupName = "Fixed Assets",
                GroupUnder = 0,
                GroupCode = "4",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 5,
                AccountGroupName = "Investments",
                GroupUnder = 0,
                GroupCode = "5",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 6,
                AccountGroupName = "Current Assets",
                GroupUnder = 0,
                GroupCode = "6",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 7,
                AccountGroupName = "Branch /Divisions",
                GroupUnder = 0,
                GroupCode = "7",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 8,
                AccountGroupName = "Misc.Expenses (ASSET)",
                GroupUnder = 0,
                GroupCode = "8",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 9,
                AccountGroupName = "Suspense A/C",
                GroupUnder = 0,
                GroupCode = "9",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 10,
                AccountGroupName = "Sales Account",
                GroupUnder = 0,
                GroupCode = "10",
                IsDefault = true,
                Nature = "Income",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 11,
                AccountGroupName = "Purchase Account",
                GroupUnder = 0,
                GroupCode = "11",
                IsDefault = true,
                Nature = "Expenses",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 12,
                AccountGroupName = "Direct Income",
                GroupUnder = 0,
                GroupCode = "12",
                IsDefault = true,
                Nature = "Income",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 13,
                AccountGroupName = "Direct Expenses",
                GroupUnder = 0,
                GroupCode = "13",
                IsDefault = true,
                Nature = "Expenses",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 14,
                AccountGroupName = "Indirect Income",
                GroupUnder = 0,
                GroupCode = "14",
                IsDefault = true,
                Nature = "Income",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 15,
                AccountGroupName = "Indirect Expenses",
                GroupUnder = 0,
                GroupCode = "15",
                IsDefault = true,
                Nature = "Expenses",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 16,
                AccountGroupName = "Reservers &Surplus",
                GroupUnder = 1,
                GroupCode = "16",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 17,
                AccountGroupName = "Bank OD A/C",
                GroupUnder = 2,
                GroupCode = "17",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 18,
                AccountGroupName = "Secured Loans",
                GroupUnder = 2,
                GroupCode = "18",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 19,
                AccountGroupName = "UnSecured Loans",
                GroupUnder = 2,
                GroupCode = "19",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 20,
                AccountGroupName = "Duties & Taxes",
                GroupUnder = 3,
                GroupCode = "20",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 21,
                AccountGroupName = "Provisions",
                GroupUnder = 3,
                GroupCode = "21",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 22,
                AccountGroupName = "Account Payable",
                GroupUnder = 3,
                GroupCode = "22",
                IsDefault = true,
                Nature = "Liabilities",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 23,
                AccountGroupName = "Stock-in-Hand",
                GroupUnder = 6,
                GroupCode = "23",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 24,
                AccountGroupName = "Deposits(Assets)",
                GroupUnder = 6,
                GroupCode = "24",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 25,
                AccountGroupName = "Loans & Advances(Asset)",
                GroupUnder = 6,
                GroupCode = "25",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 26,
                AccountGroupName = "Account Receivables",
                GroupUnder = 6,
                GroupCode = "26",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 27,
                AccountGroupName = "Cash-in Hand",
                GroupUnder = 6,
                GroupCode = "27",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 28,
                AccountGroupName = "Bank Account",
                GroupUnder = 6,
                GroupCode = "28",
                IsDefault = true,
                Nature = "Assets",
                AddedDate = DateTime.UtcNow
            });
            info.Add(new AccountGroup()
            {
                AccountGroupId = 29,
                AccountGroupName = "Service Account",
                GroupUnder = 12,
                GroupCode = "29",
                IsDefault = true,
                Nature = "Income",
                AddedDate = DateTime.UtcNow
            });
            return info;
        }
        }
}
