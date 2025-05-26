using System.ComponentModel.DataAnnotations;

namespace DanceualifiersMVC.ViewModels;

public class CreateNotificationViewModel
{
    [Required(ErrorMessage = "Заголовок обязателен")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Сообщение обязательно")]
    public string Message { get; set; }

    public string? TargetUserId { get; set; }
}