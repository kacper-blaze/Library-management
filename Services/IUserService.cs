using lab10.Models;

namespace lab10.Services;

public interface IUserService
{
    Task RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task LogoutAsync();
}  