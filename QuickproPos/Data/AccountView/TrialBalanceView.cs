namespace QuickproPos.Data.AccountView
{
    public class TrialBalanceView
    {
        public int AccountGroupId { get; set; }
        public string AccountGroupName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string OpeningBalance { get; set; }
        public string Balance { get; set; }
        public decimal OpBalance { get; set; }
        public decimal Balance1 { get; set; }
    }

    public class ProfitLossView
    {
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Balance { get; set; }
    }

}
