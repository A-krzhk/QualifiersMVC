using DanceualifiersMVC.Data;
using DanceualifiersMVC.Models;
using DanceualifiersMVC.Models.Constants;
using DanceualifiersMVC.Services;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Controllers;
[Authorize(Roles = RoleConstants.Staff)]
public class AdminController : Controller
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(AuthService authService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult CreateAdmin()
    {
        return View();
    }
    
    public async Task<IActionResult> Index()
    {
        ViewBag.DirectionCount = await _context.Directions.CountAsync();
        ViewBag.ParticipantCount = await _context.Users.CountAsync();
        ViewBag.TimeSlotCount = await _context.TimeSlots.CountAsync();
        ViewBag.RegistrationCount = await _context.Registrations.CountAsync();

        ViewBag.LatestRegistrations = await _context.Registrations
            .Include(e => e.User)
            .Include(e => e.TimeSlot)
            .ThenInclude(ts => ts.Direction)
            .OrderByDescending(e => e.RegisteredAt) 
            .Take(5)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _authService.RegisterStaffUserAsync(model); 

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Администратор успешно создан";
                return RedirectToAction("Index"); 
            }

            ModelState.AddModelError(string.Empty, result.Message);
        }

        return View(model);
    }

    public IActionResult AdminPanel()
    {
        return View(); 
    }

    // НОВЫЕ МЕТОДЫ ДЛЯ УВЕДОМЛЕНИЙ

    public async Task<IActionResult> Notifications()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Admin)
            .Include(n => n.TargetUser)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return View(notifications);
    }

    public async Task<IActionResult> CreateNotification()
    {
        var participants = await _userManager.GetUsersInRoleAsync(RoleConstants.Participant);
        ViewBag.Users = participants;
        if (!participants.Any())
        {
            TempData["ErrorMessage"] = "Не найдено участников для отправки уведомлений.";
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNotification(CreateNotificationViewModel model)
    {
        var adminId = _userManager.GetUserId(User);

        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError(string.Empty, "Пользователь не аутентифицирован.");
        }
        else if (string.IsNullOrEmpty(adminId))
        {
            ModelState.AddModelError(string.Empty, "Не удалось получить идентификатор администратора.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ErrorMessage"] = "Ошибки валидации: " + string.Join("; ", errors);
            var participants = await _userManager.GetUsersInRoleAsync(RoleConstants.Participant);
            ViewBag.Users = participants;
            return View(model);
        }

        var notification = new Notification
        {
            Title = model.Title,
            Message = model.Message,
            TargetUserId = model.TargetUserId,
            AdminId = adminId,
            CreatedAt = DateTime.Now
        };

        _context.Add(notification);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Уведомление успешно создано!";
        return RedirectToAction(nameof(Notifications));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Уведомление удалено!";
        }
        
        return RedirectToAction(nameof(Notifications));
    }
}
