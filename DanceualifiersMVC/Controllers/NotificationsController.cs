using DanceualifiersMVC.Data;
using DanceualifiersMVC.Models;
using DanceualifiersMVC.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Controllers;
[Authorize(Roles = RoleConstants.Participant)]
public class NotificationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
            
        var notifications = await _context.Notifications
            .Include(n => n.Admin)
            .Where(n => n.TargetUserId == userId || n.TargetUserId == null)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
                
        return View(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = _userManager.GetUserId(User);
        var notification = await _context.Notifications
            .Where(n => n.Id == id && (n.TargetUserId == userId || n.TargetUserId == null))
            .FirstOrDefaultAsync();
                
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
            
        return Json(new { success = true });
    }

    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = _userManager.GetUserId(User);
            
        var count = await _context.Notifications
            .Where(n => (n.TargetUserId == userId || n.TargetUserId == null) && !n.IsRead)
            .CountAsync();
                
        return Json(new { count });
    }
}