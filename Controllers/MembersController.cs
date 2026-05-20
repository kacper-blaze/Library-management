using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab10.Models;

namespace lab10.Controllers;

public class MembersController : Controller
{
    private readonly AppDbContext _dbContext;

    public MembersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetString("IsLoggedIn") == "true";
    }

    private bool IsAdmin()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        return userRole == "Admin";
    }

    // GET: Members
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var members = await _dbContext.Members.ToListAsync();
        return View(members);
    }

    // GET: Members/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var member = await _dbContext.Members
            .Include(m => m.Borrowings)
            .ThenInclude(b => b.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // GET: Members/Create
    public IActionResult Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Members/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (ModelState.IsValid)
        {
            member.MembershipDate = DateTime.Now;
            _dbContext.Add(member);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    // GET: Members/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var member = await _dbContext.Members.FindAsync(id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST: Members/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Member member)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id != member.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    // GET: Members/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST: Members/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        if (!IsAdmin())
        {
            return RedirectToAction("Index", "Home");
        }

        var member = await _dbContext.Members.FindAsync(id);
        if (member != null)
        {
            _dbContext.Members.Remove(member);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool MemberExists(int id)
    {
        return _dbContext.Members.Any(e => e.Id == id);
    }
}
