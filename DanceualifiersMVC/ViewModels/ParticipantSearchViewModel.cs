using Microsoft.AspNetCore.Mvc.Rendering;

namespace DanceualifiersMVC.ViewModels;

public class ParticipantSearchViewModel
{
    public int DirectionId { get; set; }
    public int TimeSlotId { get; set; }
    public int SeatNumber { get; set; }

    public List<SelectListItem> Directions { get; set; } = new();
    public List<SelectListItem> TimeSlots { get; set; } = new();
    public ParticipantViewModel? Result { get; set; }
}
