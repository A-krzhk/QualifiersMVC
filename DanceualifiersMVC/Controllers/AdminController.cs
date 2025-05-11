using DanceualifiersMVC.Data;
using DanceualifiersMVC.Models.Constants;
using DanceualifiersMVC.Services;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Controllers;

[Authorize(Roles = RoleConstants.Staff)]
public class AdminController : Controller
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;

    public AdminController(AuthService authService, ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
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
}
