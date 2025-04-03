namespace QuickproPos.Data.SettingView
{
    public class DataInfo
    {
        public class DateInfo
        {
            public string Name { get; set; }
            public List<DateInfo> GetDateInfo()
            {
                var dateinfos = new List<DateInfo>();
                dateinfos.Add(new DateInfo() { Name = "Today" });
                dateinfos.Add(new DateInfo() { Name = "CurrentMonth" });
                dateinfos.Add(new DateInfo() { Name = "CurrentYear" });
                dateinfos.Add(new DateInfo() { Name = "LastMonth" });
                dateinfos.Add(new DateInfo() { Name = "LastQuarter" });
                dateinfos.Add(new DateInfo() { Name = "LastYear" });
                dateinfos.Add(new DateInfo() { Name = "Last30Days" });
                dateinfos.Add(new DateInfo() { Name = "Last60Days" });
                dateinfos.Add(new DateInfo() { Name = "Last90Days" });
                dateinfos.Add(new DateInfo() { Name = "ThisFinancialYear" });
                return dateinfos;
            }
        }
    }
}
