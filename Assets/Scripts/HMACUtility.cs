using System.Security.Cryptography;
using System.Text;

public static class HMACUtility
{
    public static string GenerateSignature(string message, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(messageBytes);
        return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
