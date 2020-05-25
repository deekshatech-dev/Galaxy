using System.ComponentModel.DataAnnotations;

namespace GPSMap.Models
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}