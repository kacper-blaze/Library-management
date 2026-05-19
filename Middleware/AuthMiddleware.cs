using Microsoft.AspNetCore.Http;

namespace lab10.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        if (path.StartsWith("/Account/") || path == "/")
        {
            await _next(context);
            return;
        }

        var isLoggedIn = context.Session.GetString("IsLoggedIn");
        
        if (isLoggedIn != "true")
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        await _next(context);
    }
}