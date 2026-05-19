using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CategoriesApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CategoriesApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool Authenticate(string username, string token)
    {
        var user = _dbContext.Loginy.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var categories = await _dbContext.Categories.ToListAsync();
        return Ok(categories);
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var category = await _dbContext.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    // PUT: api/Categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (id != category.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Entry(category).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dbContext.Categories.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
