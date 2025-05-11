namespace DanceualifiersMVC.ViewModels;

public class RegistrationViewModel
{
    public int Id { get; set; }
    public string DirectionName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int SeatNumber { get; set; }
}