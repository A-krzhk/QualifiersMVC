using System.ComponentModel.DataAnnotations;

namespace DanceualifiersMVC.ViewModels;

public class DirectionViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название обязательно")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Описание обязательно")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = string.Empty;
    
    public List<TimeSlotViewModel> TimeSlots { get; set; } = new List<TimeSlotViewModel>();
}