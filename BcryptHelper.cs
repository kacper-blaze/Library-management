using BCrypt.Net;

namespace lab10;

public static class BcryptHelper
{
    public static string ComputeBCryptHash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public static bool VerifyHash(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}