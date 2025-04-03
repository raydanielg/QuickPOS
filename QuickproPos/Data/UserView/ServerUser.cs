namespace QuickproPos.Data.UserView
{
    public class ServerUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ValidDate { get; set; }
        public string LicenseKey { get; set; }
        public string MachineId { get; set; }
        public string TenantId { get; set; }
    }
}
