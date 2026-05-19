using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lab10.Models;
using Microsoft.EntityFrameworkCore;

namespace lab10.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Reports()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToAction("Login", "Account");
        }

        var dbContext = HttpContext.RequestServices.GetRequiredService<AppDbContext>();

        // Most borrowed books
        var mostBorrowedBooks = await dbContext.Borrowings
            .Where(b => b.ReturnDate != null)
            .GroupBy(b => b.BookId)
            .Select(g => new { BookId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync();

        var topBooks = new List<object>();
        foreach (var item in mostBorrowedBooks)
        {
            var book = await dbContext.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == item.BookId);
            if (book != null)
            {
                topBooks.Add(new { Title = book.Title, Author = book.Author?.FirstName + " " + book.Author?.LastName, Count = item.Count });
            }
        }

        // Overdue borrowings
        var overdueBorrowings = await dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .Where(b => b.ReturnDate == null && b.DueDate < DateTime.Now)
            .ToListAsync();

        // Popular authors
        var popularAuthors = await dbContext.Borrowings
            .Where(b => b.ReturnDate != null)
            .GroupBy(b => b.Book.AuthorId)
            .Select(g => new { AuthorId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync();

        var topAuthors = new List<object>();
        foreach (var item in popularAuthors)
        {
            var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == item.AuthorId);
            if (author != null)
            {
                topAuthors.Add(new { Name = author.FirstName + " " + author.LastName, Count = item.Count });
            }
        }

        // Available books by category
        var booksByCategory = await dbContext.Categories
            .Select(c => new
            {
                CategoryName = c.Name,
                TotalBooks = c.Books.Count,
                AvailableBooks = c.Books.Sum(b => b.AvailableCopies)
            })
            .ToListAsync();

        ViewBag.MostBorrowedBooks = topBooks;
        ViewBag.OverdueBorrowings = overdueBorrowings;
        ViewBag.PopularAuthors = topAuthors;
        ViewBag.BooksByCategory = booksByCategory;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
