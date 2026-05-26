using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

[ApiController]
[Route("api/borrowings")]
public class BorrowingsApiController : BaseApiController
{
    public BorrowingsApiController(AppDbContext dbContext, ILogger<BorrowingsApiController> logger) : base(dbContext, logger)
    {
    }

    // GET: api/borrowings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Borrowing>>> GetBorrowings()
    {
        Logger.LogInformation("GET api/borrowings called");
        if (!IsAuthenticated())
            return Unauthorized();

        var borrowings = await DbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .ToListAsync();
        return Ok(borrowings);
    }

    // GET: api/borrowings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Borrowing>> GetBorrowing(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var borrowing = await DbContext.Borrowings
            .Include(b => b.Book)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrowing == null)
            return NotFound();

        return Ok(borrowing);
    }

    // POST: api/borrowings
    [HttpPost]
    public async Task<ActionResult<Borrowing>> CreateBorrowing(Borrowing borrowing)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if book is available
        var book = await DbContext.Books.FindAsync(borrowing.BookId);
        if (book == null || book.AvailableCopies <= 0)
            return BadRequest("Book is not available");

        // Set borrow date if not provided
        if (borrowing.BorrowDate == default)
            borrowing.BorrowDate = DateTime.Now;

        // Set due date if not provided (14 days from borrow date)
        if (borrowing.DueDate == default)
            borrowing.DueDate = borrowing.BorrowDate.AddDays(14);

        // Decrease available copies
        book.AvailableCopies--;
        DbContext.Entry(book).State = EntityState.Modified;

        DbContext.Borrowings.Add(borrowing);
        await DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBorrowing), new { id = borrowing.Id }, borrowing);
    }

    // PUT: api/borrowings/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBorrowing(int id, Borrowing borrowing)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (id != borrowing.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Entry(borrowing).State = EntityState.Modified;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BorrowingExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/borrowings/5/return
    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var borrowing = await DbContext.Borrowings.FindAsync(id);
        if (borrowing == null)
            return NotFound();

        if (borrowing.ReturnDate != null)
            return BadRequest("Book already returned");

        // Set return date
        borrowing.ReturnDate = DateTime.Now;

        // Increase available copies
        var book = await DbContext.Books.FindAsync(borrowing.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
            DbContext.Entry(book).State = EntityState.Modified;
        }

        DbContext.Entry(borrowing).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/borrowings/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBorrowing(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var borrowing = await DbContext.Borrowings.FindAsync(id);
        if (borrowing == null)
            return NotFound();

        // If book was not returned, increase available copies
        if (borrowing.ReturnDate == null)
        {
            var book = await DbContext.Books.FindAsync(borrowing.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
                DbContext.Entry(book).State = EntityState.Modified;
            }
        }

        DbContext.Borrowings.Remove(borrowing);
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool BorrowingExists(int id)
    {
        return DbContext.Borrowings.Any(e => e.Id == id);
    }
}
