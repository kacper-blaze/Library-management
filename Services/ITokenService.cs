using lab10.Models;

namespace lab10.Services;

public interface ITokenService
{
    string GenerateToken(Login user);
}