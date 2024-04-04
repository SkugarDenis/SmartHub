using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartHub.Models
{
    public class AddNewUserViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [UIHint("password")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
