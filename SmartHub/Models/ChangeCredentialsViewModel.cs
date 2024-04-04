using System.ComponentModel.DataAnnotations;

namespace SmartHub.Models
{
    public class ChangeCredentialsViewModel
    {


        [Required(ErrorMessage = "Поле \"Старый пароль\" обязательно для заполнения")]
        [UIHint("password")]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Поле \"Новый пароль\" обязательно для заполнения")]
        [UIHint("password")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [UIHint("password")]
        [Display(Name = "Повторите пароль")]
        public string NewPasswordSecond { get; set; }
    }
}