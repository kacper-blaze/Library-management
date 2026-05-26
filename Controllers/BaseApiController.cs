using Microsoft.AspNetCore.Mvc;
using lab10.Models;

namespace lab10.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected readonly AppDbContext DbContext;
    protected readonly ILogger Logger;

    protected BaseApiController(AppDbContext dbContext, ILogger logger)
    {
        DbContext = dbContext;
        Logger = logger;
    }

    protected bool IsAuthenticated()
    {
        if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("token"))
        {
            Logger.LogWarning("API Auth failed: Missing headers. Username: {HasUsername}, Token: {HasToken}", 
                Request.Headers.ContainsKey("username"), Request.Headers.ContainsKey("token"));
            return false;
        }

        var username = Request.Headers["username"].ToString();
        var token = Request.Headers["token"].ToString();

        var user = DbContext.Loginy.FirstOrDefault(l => l.Username == username && l.ApiToken == token);
        
        if (user == null)
        {
            Logger.LogWarning("API Auth failed: Invalid credentials for user {Username}", username);
            return false;
        }

        return true;
    }
}
