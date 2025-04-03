namespace QuickproPos.Data.SettingView
{
    public class ProductGroupView
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        public int GroupUnder { get; set; }
        public string Under { get; set; }

        public string Image { get; set; }
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
