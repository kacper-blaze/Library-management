using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _dbContext;

    public CategoriesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetString("IsLoggedIn") == "true";
    }

    private bool IsAdmin()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        return userRole == "Admin";
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var categories = await _dbContext.Categories.ToListAsync();
        return View(categories);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var category = await _dbContext.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (ModelState.IsValid)
        {
            _dbContext.Add(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(category);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var category = await _dbContext.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var category = await _dbContext.Categories.FindAsync(id);
        if (category != null)
        {
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _dbContext.Categories.Any(e => e.Id == id);
    }
}
