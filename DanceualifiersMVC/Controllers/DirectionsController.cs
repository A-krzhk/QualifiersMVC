using DanceualifiersMVC.Models;
using DanceualifiersMVC.Models.Constants;
using DanceualifiersMVC.Services;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DanceualifiersMVC.Controllers;

public class DirectionsController : Controller
{
    private readonly DirectionService _directionService;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public DirectionsController(DirectionService directionService, UserManager<ApplicationUser> userManager)
    {
        _directionService = directionService;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var directions = await _directionService.GetAllDirectionsAsync();
        return View(directions);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var direction = await _directionService.GetDirectionByIdAsync(id);
            return View(direction);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Staff)]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DirectionViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _directionService.UpdateDirectionAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        
        return View(model);
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DirectionViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _directionService.CreateDirectionAsync(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        
        return View(model);
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Staff)]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var direction = await _directionService.GetDirectionByIdAsync(id);
            return View(direction);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _directionService.DeleteDirectionAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Staff)]
    public async Task<IActionResult> AddTimeSlot(int directionId)
    {
        try
        {
            var direction = await _directionService.GetDirectionByIdAsync(directionId);
            ViewBag.Direction = direction;
            return View(new TimeSlotViewModel { DirectionId = directionId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTimeSlot(TimeSlotViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _directionService.AddTimeSlotAsync(model.DirectionId, model);
                return RedirectToAction(nameof(Details), new { id = model.DirectionId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        
        try
        {
            var direction = await _directionService.GetDirectionByIdAsync(model.DirectionId);
            ViewBag.Direction = direction;
        }
        catch
        {
            return NotFound();
        }
        
        return View(model);
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTimeSlot(int id, int directionId)
    {
        try
        {
            await _directionService.DeleteTimeSlotAsync(id);
            return RedirectToAction(nameof(Details), new { id = directionId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}