using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthorsApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AuthorsApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool Authenticate(string username, string token)
    {
        var user = _dbContext.Loginy.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null;
    }

    // GET: api/Authors
    [HttpGet]
    public async Task<IActionResult> GetAuthors([FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var authors = await _dbContext.Authors.ToListAsync();
        return Ok(authors);
    }

    // GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var author = await _dbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
            return NotFound();

        return Ok(author);
    }

    // POST: api/Authors
    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] Author author, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Authors.Add(author);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    // PUT: api/Authors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author author, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (id != author.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Entry(author).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dbContext.Authors.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var author = await _dbContext.Authors.FindAsync(id);
        if (author == null)
            return NotFound();

        _dbContext.Authors.Remove(author);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
