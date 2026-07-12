using System.Security.Cryptography;

public class RefreshTokenService
{

    public string Generate()
    {
        var randomBytes = new byte[64];

        using var generator = RandomNumberGenerator.Create();

        generator.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }


    public DateTime GetExpiryDate()
    {
        return DateTime.UtcNow.AddDays(30);
    }

}
