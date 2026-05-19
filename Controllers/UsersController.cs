using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using lab10.Models;
using lab10.Services;

namespace lab10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IUserService _userService;

    public UsersController(AppDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _userService.RegisterAsync(model.Email, model.Password);
            return Ok(new { message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return StatusCode(500, ModelState);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var token = await _userService.LoginAsync(model.Email, model.Password);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return StatusCode(500, ModelState);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _userService.LogoutAsync();
            return Ok(new { message = "User logged out successfully" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return StatusCode(500, ModelState);
        }
    }
}