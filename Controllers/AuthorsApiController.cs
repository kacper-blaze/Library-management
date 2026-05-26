using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsApiController : BaseApiController
{
    public AuthorsApiController(AppDbContext dbContext, ILogger<AuthorsApiController> logger) : base(dbContext, logger)
    {
    }

    // GET: api/authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
        Logger.LogInformation("GET api/authors called");
        if (!IsAuthenticated())
            return Unauthorized();

        var authors = await DbContext.Authors
            .Include(a => a.Books)
            .ToListAsync();
        return Ok(authors);
    }

    // GET: api/authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> GetAuthor(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var author = await DbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
            return NotFound();

        return Ok(author);
    }

    // POST: api/authors
    [HttpPost]
    public async Task<ActionResult<Author>> CreateAuthor(Author author)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Authors.Add(author);
        await DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    // PUT: api/authors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, Author author)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (id != author.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Entry(author).State = EntityState.Modified;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var author = await DbContext.Authors.FindAsync(id);
        if (author == null)
            return NotFound();

        DbContext.Authors.Remove(author);
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool AuthorExists(int id)
    {
        return DbContext.Authors.Any(e => e.Id == id);
    }
}
