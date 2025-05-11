using System.ComponentModel.DataAnnotations;

namespace DanceualifiersMVC.ViewModels;

public class TimeSlotViewModel
{
    public int Id { get; set; }
    
    public int DirectionId { get; set; }
    
    [Required(ErrorMessage = "Время начала обязательно")]
    [Display(Name = "Время начала")]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }
    
    [Required(ErrorMessage = "Время окончания обязательно")]
    [Display(Name = "Время окончания")]
    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }
    
    [Required(ErrorMessage = "Максимальное количество участников обязательно")]
    [Display(Name = "Максимальное количество участников")]
    [Range(1, 100, ErrorMessage = "Количество участников должно быть от 1 до 100")]
    public int MaxParticipants { get; set; }
    
    public int AvailableSeats { get; set; }
}