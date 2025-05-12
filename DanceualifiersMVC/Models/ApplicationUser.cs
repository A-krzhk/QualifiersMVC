using Microsoft.AspNetCore.Identity;

namespace DanceualifiersMVC.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}