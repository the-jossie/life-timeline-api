using System.Security.Cryptography;
using System.Text;

public class RefreshTokenService
{
    public string Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }


    public string Hash(string token)
    {
        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
