using DanceualifiersMVC.Models;
using DanceualifiersMVC.Models.Constants;
using DanceualifiersMVC.Settings;
using DanceualifiersMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace DanceualifiersMVC.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            return (false, "Пользователь с таким email уже существует");
        }

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Telegram = model.Telegram ?? string.Empty,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Ошибка при создании пользователя: {errors}");
        }

        // Назначение роли
        var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.Participant);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            return (false, $"Ошибка при назначении роли: {errors}");
        }

        return (true, "Пользователь успешно зарегистрирован");
    }
    
    public async Task<(bool Success, string Message)> RegisterStaffUserAsync(RegisterViewModel model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            return (false, "Пользователь с таким email уже существует");
        }

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Telegram = model.Telegram ?? string.Empty,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Ошибка при создании пользователя: {errors}");
        }

        // Назначение роли
        var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.Staff);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            return (false, $"Ошибка при назначении роли: {errors}");
        }

        return (true, "Пользователь успешно зарегистрирован");
    }

    public async Task<(bool Success, string Message)> LoginAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user == null)
        {
            return (false, "Неверный email или пароль");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return (false, "Неверный email или пароль");
        }

        return (true, "Вход выполнен успешно");
    }
    
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}