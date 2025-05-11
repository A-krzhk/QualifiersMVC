using DanceualifiersMVC.Data;
using DanceualifiersMVC.Models;
using DanceualifiersMVC.Models.Constants;
using DanceualifiersMVC.Services;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Controllers;

[Authorize]
public class RegistrationsController : Controller
{
    private readonly RegistrationService _registrationService;
    private readonly DirectionService _directionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    
    public RegistrationsController(
        RegistrationService registrationService, 
        DirectionService directionService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _registrationService = registrationService;
        _directionService = directionService;
        _userManager = userManager;
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> MyRegistrations()
    {
        var userId = _userManager.GetUserId(User);
        var registrations = await _registrationService.GetUserRegistrationsAsync(userId);
        return View(registrations);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int timeSlotId, int directionId)
    {
        var userId = _userManager.GetUserId(User);
        
        try
        {
            await _registrationService.RegisterForTimeSlotAsync(userId, timeSlotId);
            TempData["SuccessMessage"] = "Вы успешно зарегистрировались";
            return RedirectToAction(nameof(MyRegistrations));
        }
        catch (KeyNotFoundException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction("Details", "Directions", new { id = directionId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = _userManager.GetUserId(User);
        
        try
        {
            await _registrationService.CancelRegistrationAsync(userId, id);
            TempData["SuccessMessage"] = "Регистрация отменена";
        }
        catch (KeyNotFoundException)
        {
            TempData["ErrorMessage"] = "Запись не найдена";
        }
        
        return RedirectToAction(nameof(MyRegistrations));
    }

    [HttpGet]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetTimeSlots(int directionId)
    {
        var timeSlots = await _context.TimeSlots
            .Where(ts => ts.DirectionId == directionId)
            .Select(ts => new SelectListItem
            {
                Value = ts.Id.ToString(),
                Text = $"{ts.StartTime:dd.MM.yyyy HH:mm} - {ts.EndTime:HH:mm}"
            })
            .ToListAsync();

        timeSlots.Insert(0, new SelectListItem { Value = "", Text = "Выберите временной слот", Disabled = true });

        return Json(timeSlots);
    }
    
    
    [HttpGet]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> FindParticipant()
    {
        var directions = await _context.Directions
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            .ToListAsync();

        var viewModel = new ParticipantSearchViewModel
        {
            Directions = directions
        };
        directions.Insert(0, new SelectListItem
        {
            Value = "",
            Text = "Выберите направление",
            Disabled = false,
            Selected = true
        });

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Staff")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FindParticipant(ParticipantSearchViewModel model)
    {
        model.Directions = await _context.Directions
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            .ToListAsync();

        model.TimeSlots = await _context.TimeSlots
            .Where(ts => ts.DirectionId == model.DirectionId)
            .Select(ts => new SelectListItem
            {
                Value = ts.Id.ToString(),
                Text = $"{ts.StartTime:dd.MM.yyyy HH:mm} - {ts.EndTime:HH:mm}"
            })
            .ToListAsync();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            model.Result = await _registrationService.FindParticipantAsync(model.DirectionId, model.TimeSlotId, model.SeatNumber);
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }
    
    [HttpGet]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Participants(int timeSlotId)
    {
        var timeSlot = await _context.TimeSlots
            .Include(ts => ts.Direction)
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

        if (timeSlot == null)
            return NotFound();

        var participants = await _registrationService.GetParticipantsByTimeSlotAsync(timeSlotId);

        ViewBag.DirectionName = timeSlot.Direction?.Name ?? "Не указано";
        ViewBag.StartTime = timeSlot.StartTime;
        ViewBag.EndTime = timeSlot.EndTime;
        ViewBag.DirectionId = timeSlot.DirectionId;

        return View(participants);
    }

    
    

}