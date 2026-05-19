using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

public class BooksController : Controller
{
    private readonly AppDbContext _dbContext;

    public BooksController(AppDbContext dbContext)
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

    // GET: Books
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var books = await _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .ToListAsync();
        return View(books);
    }

    // GET: Books/Details/5
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

        var book = await _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: Books/Create
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

        ViewBag.Authors = _dbContext.Authors.ToList();
        ViewBag.Categories = _dbContext.Categories.ToList();
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
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
            book.AvailableCopies = book.TotalCopies;
            _dbContext.Add(book);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Authors = _dbContext.Authors.ToList();
        ViewBag.Categories = _dbContext.Categories.ToList();
        return View(book);
    }

    // GET: Books/Edit/5
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

        var book = await _dbContext.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        ViewBag.Authors = _dbContext.Authors.ToList();
        ViewBag.Categories = _dbContext.Categories.ToList();
        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(book);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
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

        ViewBag.Authors = _dbContext.Authors.ToList();
        ViewBag.Categories = _dbContext.Categories.ToList();
        return View(book);
    }

    // GET: Books/Delete/5
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

        var book = await _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: Books/Delete/5
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

        var book = await _dbContext.Books.FindAsync(id);
        if (book != null)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int id)
    {
        return _dbContext.Books.Any(e => e.Id == id);
    }
}
