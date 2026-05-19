using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

public class AuthorsController : Controller
{
    private readonly AppDbContext _dbContext;

    public AuthorsController(AppDbContext dbContext)
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

    // GET: Authors
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var authors = await _dbContext.Authors.ToListAsync();
        return View(authors);
    }

    // GET: Authors/Details/5
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

        var author = await _dbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    // GET: Authors/Create
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

    // POST: Authors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Author author)
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
            _dbContext.Add(author);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    // GET: Authors/Edit/5
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

        var author = await _dbContext.Authors.FindAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    // POST: Authors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Author author)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id != author.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(author);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(author.Id))
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
        return View(author);
    }

    // GET: Authors/Delete/5
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

        var author = await _dbContext.Authors.FirstOrDefaultAsync(m => m.Id == id);
        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    // POST: Authors/Delete/5
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

        var author = await _dbContext.Authors.FindAsync(id);
        if (author != null)
        {
            _dbContext.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AuthorExists(int id)
    {
        return _dbContext.Authors.Any(e => e.Id == id);
    }
}
