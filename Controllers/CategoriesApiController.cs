using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesApiController : BaseApiController
{
    public CategoriesApiController(AppDbContext dbContext, ILogger<CategoriesApiController> logger) : base(dbContext, logger)
    {
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        Logger.LogInformation("GET api/categories called");
        if (!IsAuthenticated())
            return Unauthorized();

        var categories = await DbContext.Categories
            .Include(c => c.Books)
            .ToListAsync();
        return Ok(categories);
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var category = await DbContext.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (id != category.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Entry(category).State = EntityState.Modified;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var category = await DbContext.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        DbContext.Categories.Remove(category);
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return DbContext.Categories.Any(e => e.Id == id);
    }
}
