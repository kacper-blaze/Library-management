using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class BooksApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public BooksApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool Authenticate(string username, string token)
    {
        var user = _dbContext.Loginy.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var books = await _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var book = await _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    // POST: api/Books
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] Book book, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        book.AvailableCopies = book.TotalCopies;
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (id != book.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Entry(book).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dbContext.Books.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var book = await _dbContext.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
