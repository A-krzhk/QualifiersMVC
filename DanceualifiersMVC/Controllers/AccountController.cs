using DanceualifiersMVC.Models;
using DanceualifiersMVC.Services;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DanceualifiersMVC.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(AuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _authService.RegisterAsync(model);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Регистрация прошла успешно. Теперь вы можете войти.";
                return RedirectToAction("Login");
            }
            
            ModelState.AddModelError(string.Empty, result.Message);
        }
        
        return View(model);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (ModelState.IsValid)
        {
            var result = await _authService.LoginAsync(model);
            
            if (result.Success)
            {
                return RedirectToLocal(returnUrl);
            }
            
            ModelState.AddModelError(string.Empty, result.Message);
        }
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Directions");
    }
    
    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Directions");
        }
    }
}