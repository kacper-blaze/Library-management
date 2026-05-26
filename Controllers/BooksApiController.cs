using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

[ApiController]
[Route("api/books")]
public class BooksApiController : BaseApiController
{
    public BooksApiController(AppDbContext dbContext, ILogger<BooksApiController> logger) 
        : base(dbContext, logger)
    {
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        Logger.LogInformation("GET api/books called");
        if (!IsAuthenticated())
            return Unauthorized();

        var books = await DbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .ToListAsync();
        return Ok(books);
    }

    // GET: api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var book = await DbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        book.AvailableCopies = book.TotalCopies;
        DbContext.Books.Add(book);
        await DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PUT: api/books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (id != book.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Entry(book).State = EntityState.Modified;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var book = await DbContext.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        DbContext.Books.Remove(book);
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return DbContext.Books.Any(e => e.Id == id);
    }
}
