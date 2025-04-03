using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace QuickproPos.Data.Setting
{
    public class Privilege
    {
        [PrimaryKey, AutoIncrement]
        public int PrivilegeId { get; set; }

        [Required]
        [MaxLength(255)] // SQLite does not enforce string length, but good practice
        public string FormName { get; set; }

        public bool AddAction { get; set; }
        public bool EditAction { get; set; }
        public bool DeleteAction { get; set; }
        public bool ShowAction { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }

        [MaxLength(50)]
        public string SettingType { get; set; }
    }
}
