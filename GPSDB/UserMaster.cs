using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSDB
{
    public class UserMaster
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and Compare password should be equal")]
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
