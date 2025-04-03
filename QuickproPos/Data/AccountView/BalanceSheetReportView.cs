namespace QuickproPos.Data.AccountView
{
    public class BalanceSheetReportView
    {
        public Dictionary<string, decimal> Assets { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> Liabilities { get; set; } = new Dictionary<string, decimal>();
        public decimal Equity { get; set; }
        public decimal TotalAssets => Assets.Values.Sum();
        public decimal TotalLiabilitiesAndEquity => Liabilities.Values.Sum() + Equity;
    }

}
