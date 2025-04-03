using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class EmailSetting
    {
        [PrimaryKey, AutoIncrement]
        public int EmailSettingId { get; set; }

        [Required]
        [MaxLength(256)]  // Ensure MailHost is within a reasonable size
        public string MailHost { get; set; } = string.Empty;

        [Required]
        public int MailPort { get; set; } = 0;

        [Required]
        [MaxLength(256)]  // Limit the length of MailAddress
        public string MailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]  // Limit the length of Password
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]  // Limit the length of MailFromname
        public string MailFromname { get; set; } = string.Empty;
        [MaxLength(50)]  // Encryptions names are typically short
        public string EncryptionName { get; set; } = string.Empty;
        [MaxLength(512)]  // WhatsAppApi might have a URL or API key
        public string WhatsAppApi { get; set; } = string.Empty;
        [MaxLength(15)]  // MobileNo typically has a limited length
        public string MobileNo { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
