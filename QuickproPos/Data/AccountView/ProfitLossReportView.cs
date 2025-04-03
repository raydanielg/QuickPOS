namespace QuickproPos.Data.AccountView
{
    public class ProfitLossReportView
    {
        //public decimal TotalIncome { get; set; }  // Total income
        //public decimal TotalExpenses { get; set; } // Total expenses
        //public decimal ProfitOrLoss { get; set; }  // Profit or loss (Total Income - Total Expenses)
        //public List<ProfitLossDetail> ProfitLossDetails { get; set; } // List of detailed ledger info
        public Dictionary<string, decimal> Incomes { get; set; }
        public Dictionary<string, decimal> Expenses { get; set; }
        public decimal NetProfit { get; set; }
    }

}
