using System.ComponentModel.DataAnnotations;

namespace DanceualifiersMVC.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символов", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Telegram обязателен")]
    [Display(Name = "Telegram")]
    public string Telegram { get; set; }
}