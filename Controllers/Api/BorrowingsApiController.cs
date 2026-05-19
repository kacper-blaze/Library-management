using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class BorrowingsApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public BorrowingsApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool Authenticate(string username, string token)
    {
        var user = _dbContext.Loginy.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null;
    }

    // GET: api/Borrowings
    [HttpGet]
    public async Task<IActionResult> GetBorrowings([FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var borrowings = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .ToListAsync();

        return Ok(borrowings);
    }

    // GET: api/Borrowings/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBorrowing(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var borrowing = await _dbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrowing == null)
            return NotFound();

        return Ok(borrowing);
    }

    // POST: api/Borrowings
    [HttpPost]
    public async Task<IActionResult> CreateBorrowing([FromBody] Borrowing borrowing, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var book = await _dbContext.Books.FindAsync(borrowing.BookId);
        if (book == null || book.AvailableCopies <= 0)
            return BadRequest("Book is not available");

        book.AvailableCopies--;
        borrowing.BorrowDate = DateTime.Now;
        borrowing.DueDate = DateTime.Now.AddDays(14);

        _dbContext.Borrowings.Add(borrowing);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBorrowing), new { id = borrowing.Id }, borrowing);
    }

    // PUT: api/Borrowings/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBorrowing(int id, [FromBody] Borrowing borrowing, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (id != borrowing.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Entry(borrowing).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dbContext.Borrowings.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Borrowings/5/return
    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var borrowing = await _dbContext.Borrowings.FindAsync(id);
        if (borrowing == null)
            return NotFound();

        if (borrowing.ReturnDate != null)
            return BadRequest("Book already returned");

        borrowing.ReturnDate = DateTime.Now;

        var book = await _dbContext.Books.FindAsync(borrowing.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(borrowing);
    }

    // DELETE: api/Borrowings/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBorrowing(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var borrowing = await _dbContext.Borrowings.FindAsync(id);
        if (borrowing == null)
            return NotFound();

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

        return NoContent();
    }
}
