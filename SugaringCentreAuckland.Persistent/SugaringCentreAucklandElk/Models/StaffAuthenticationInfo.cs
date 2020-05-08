using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public class StaffAuthenticationInfo
    {
        [Column("AuthenticationInfo")] 
        public int AuthenticationInfoId { get; set; }

        public string Username { get; set; }

        public string EncryptedPassword { get; set; }

        [Column("Staff")]
        public int StaffId { get; set; }
    }
}