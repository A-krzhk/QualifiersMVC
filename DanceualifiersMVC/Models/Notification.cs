using System.ComponentModel.DataAnnotations;

namespace DanceualifiersMVC.Models;

public class Notification
{
    public bool IsRead { get; set; } = false;
        
    public DateTime? ReadAt { get; set; }
    
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Сообщение обязательно")]
    public string Message { get; set; }

    public string? TargetUserId { get; set; } // Nullable, если уведомление может быть для всех
    public ApplicationUser? TargetUser { get; set; }

    public string AdminId { get; set; }
    public ApplicationUser Admin { get; set; }

    public DateTime CreatedAt { get; set; }
}