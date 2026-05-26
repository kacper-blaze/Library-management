using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

[ApiController]
[Route("api/members")]
public class MembersApiController : BaseApiController
{
    public MembersApiController(AppDbContext dbContext, ILogger<MembersApiController> logger) : base(dbContext, logger)
    {
    }

    // GET: api/members
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
    {
        Logger.LogInformation("GET api/members called");
        if (!IsAuthenticated())
            return Unauthorized();

        var members = await DbContext.Members
            .Include(m => m.Borrowings)
            .ToListAsync();
        return Ok(members);
    }

    // GET: api/members/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var member = await DbContext.Members
            .Include(m => m.Borrowings)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
            return NotFound();

        return Ok(member);
    }

    // POST: api/members
    [HttpPost]
    public async Task<ActionResult<Member>> CreateMember(Member member)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Members.Add(member);
        await DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
    }

    // PUT: api/members/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(int id, Member member)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        if (id != member.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        DbContext.Entry(member).State = EntityState.Modified;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MemberExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/members/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        if (!IsAuthenticated())
            return Unauthorized();

        var member = await DbContext.Members.FindAsync(id);
        if (member == null)
            return NotFound();

        DbContext.Members.Remove(member);
        await DbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool MemberExists(int id)
    {
        return DbContext.Members.Any(e => e.Id == id);
    }
}
