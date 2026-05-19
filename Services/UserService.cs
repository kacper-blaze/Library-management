using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

using lab10.Models;
using lab10.Services;

namespace lab10.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public UserService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task RegisterAsync(string email, string password)
    {
        var existingUser = await _dbContext.Loginy.FirstOrDefaultAsync(u => u.Username == email);

        if (existingUser != null)
            throw new Exception("Username already exists.");

        var hashedPassword = BcryptHelper.ComputeBCryptHash(password);
        var newUser = new Login
        {
            Username = email,
            Password = hashedPassword,
            Role = "User",
            ApiToken = Guid.NewGuid().ToString()
        };

        _dbContext.Loginy.Add(newUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _dbContext.Loginy.FirstOrDefaultAsync(u => u.Username == email);

        if (user == null || !BcryptHelper.VerifyHash(password, user.Password))
            throw new Exception("Invalid credentials.");

        return _tokenService.GenerateToken(user);
    }

    public async Task LogoutAsync()
    {
        // Implement logout logic here if needed
    }
}