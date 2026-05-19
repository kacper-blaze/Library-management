using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class MembersApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public MembersApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool Authenticate(string username, string token)
    {
        var user = _dbContext.Loginy.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null;
    }

    // GET: api/Members
    [HttpGet]
    public async Task<IActionResult> GetMembers([FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var members = await _dbContext.Members.ToListAsync();
        return Ok(members);
    }

    // GET: api/Members/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMember(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var member = await _dbContext.Members
            .Include(m => m.Borrowings)
            .ThenInclude(b => b.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
            return NotFound();

        return Ok(member);
    }

    // POST: api/Members
    [HttpPost]
    public async Task<IActionResult> CreateMember([FromBody] Member member, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        member.MembershipDate = DateTime.Now;
        _dbContext.Members.Add(member);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
    }

    // PUT: api/Members/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(int id, [FromBody] Member member, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        if (id != member.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _dbContext.Entry(member).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dbContext.Members.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Members/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id, [FromHeader] string username, [FromHeader] string token)
    {
        if (!Authenticate(username, token))
            return Unauthorized();

        var member = await _dbContext.Members.FindAsync(id);
        if (member == null)
            return NotFound();

        _dbContext.Members.Remove(member);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
