using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using lab10.Models;
using lab10.Services;

namespace lab10.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _dbContext;

    public AccountController(
        ILogger<AccountController> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    private bool IsAdmin()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
            return false;

        var user = _dbContext.Loginy.FirstOrDefault(l => l.Username == userEmail);
        return user != null && user.Role == "Admin";
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetString("IsLoggedIn") == "true";
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _dbContext.Loginy
                .FirstOrDefaultAsync(l => l.Username == model.Email);
            
            if (user != null && BcryptHelper.VerifyHash(model.Password, user.Password))
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserEmail", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("LoggedIn", "Account");
            }
            ModelState.AddModelError("", "Invalid login attempt");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
            {
                if (model.Password == model.ConfirmPassword)
                {
                    // Check if user already exists
                    var existingUser = await _dbContext.Loginy
                        .FirstOrDefaultAsync(l => l.Username == model.Email);
                    
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("", "Username already exists");
                        return View(model);
                    }
                
                    // Add new user to database
                    var hashedPassword = BcryptHelper.ComputeBCryptHash(model.Password);
                    var apiToken = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
                    var newUser = new Login
                    {
                        Username = model.Email,
                        Password = hashedPassword,
                        Role = "User",
                        ApiToken = apiToken
                    };
                
                    _dbContext.Loginy.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    HttpContext.Session.SetString("UserRole", newUser.Role);
                    return RedirectToAction("LoggedIn", "Account");
                }
                ModelState.AddModelError("", "Passwords do not match");
            }
            else
            {
                ModelState.AddModelError("", "Please fill in all fields");
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    
    [HttpGet]
    public async Task<IActionResult> LoggedIn()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login");
        }
        
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var user = await _dbContext.Loginy.FirstOrDefaultAsync(l => l.Username == userEmail);
        
        ViewBag.UserRole = user?.Role;
        ViewBag.ApiToken = user?.ApiToken;
        
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        if (!IsLoggedIn() || !IsAdmin())
        {
            return RedirectToAction("Login");
        }
        
        var users = await _dbContext.Loginy.ToListAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        if (!IsLoggedIn() || !IsAdmin())
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if (!IsLoggedIn() || !IsAdmin())
        {
            return RedirectToAction("Login");
        }

        if (ModelState.IsValid)
        {
            var existingUser = await _dbContext.Loginy
                .FirstOrDefaultAsync(l => l.Username == model.Username);
            
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(model);
            }
            
            var hashedPassword = BcryptHelper.ComputeBCryptHash(model.Password);
            var apiToken = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
            
            var newUser = new Login
            {
                Username = model.Username,
                Password = hashedPassword,
                Role = model.Role,
                ApiToken = apiToken
            };
            
            _dbContext.Loginy.Add(newUser);
            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction("Users");
        }
        return View(model);
    }
}