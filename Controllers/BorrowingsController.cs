using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

public class BorrowingsController : Controller
{
    private readonly AppDbContext _dbContext;

    public BorrowingsController(AppDbContext dbContext)
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

    // GET: Borrowings
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var borrowings = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .ToListAsync();
        return View(borrowings);
    }

    // GET: Borrowings/Details/5
    public async Task<IActionResult> Details(int? id)
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

        var borrowing = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (borrowing == null)
        {
            return NotFound();
        }

        return View(borrowing);
    }

    // GET: Borrowings/Create
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

        ViewBag.Books = _dbContext.Books.Where(b => b.AvailableCopies > 0).ToList();
        ViewBag.Members = _dbContext.Members.ToList();
        return View();
    }

    // POST: Borrowings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Borrowing borrowing)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var book = await _dbContext.Books.FindAsync(borrowing.BookId);
        if (book == null || book.AvailableCopies <= 0)
        {
            ModelState.AddModelError("", "Book is not available");
            ViewBag.Books = _dbContext.Books.Where(b => b.AvailableCopies > 0).ToList();
            ViewBag.Members = _dbContext.Members.ToList();
            return View(borrowing);
        }

        book.AvailableCopies--;
        borrowing.BorrowDate = DateTime.Now;
        borrowing.DueDate = DateTime.Now.AddDays(28); // 28 days loan period

        _dbContext.Add(borrowing);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Borrowings/Return/5
    public async Task<IActionResult> Return(int? id)
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

        var borrowing = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (borrowing == null)
        {
            return NotFound();
        }

        return View(borrowing);
    }

    // POST: Borrowings/Return/5
    [HttpPost, ActionName("Return")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnConfirmed(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var borrowing = await _dbContext.Borrowings.FindAsync(id);
        if (borrowing != null)
        {
            borrowing.ReturnDate = DateTime.Now;
            
            var book = await _dbContext.Books.FindAsync(borrowing.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
            }

            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Borrowings/Delete/5
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

        var borrowing = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (borrowing == null)
        {
            return NotFound();
        }

        return View(borrowing);
    }

    // POST: Borrowings/Delete/5
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

        var borrowing = await _dbContext.Borrowings.FindAsync(id);
        if (borrowing != null)
        {
            // Restore available copies if not returned
            if (borrowing.ReturnDate == null)
            {
                var book = await _dbContext.Books.FindAsync(borrowing.BookId);
                if (book != null)
                {
                    book.AvailableCopies++;
                }
            }

            _dbContext.Borrowings.Remove(borrowing);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
