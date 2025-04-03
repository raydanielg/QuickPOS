namespace QuickproPos.Data.AccountView
{
    public class AccountGroupView
    {
        public int AccountGroupId { get; set; }

        public string AccountGroupName { get; set; } = string.Empty;

        public int GroupUnder { get; set; }
        public string Under { get; set; }

        public string GroupCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public string Nature { get; set; } = string.Empty;

        public DateTime? AddedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifyDate { get; set; }
    }
}
