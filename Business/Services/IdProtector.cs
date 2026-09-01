using System.Security.Cryptography;
using System.Text;

namespace ASPProjects.Business.Services;

public class IdProtector : IIdProtector
{
    private readonly byte[] _key;

    public IdProtector(IConfiguration configuration)
    {
        var secret = configuration["JWT_SECRET"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? "DefaultEncryptionKeyMustBe32BytesLong!";

        // Derive consistent 256-bit AES key using SHA256
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
    }

    public string Encode(int id)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        var plainBytes = BitConverter.GetBytes(id);

        using var encryptor = aes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Combine IV (16 bytes) + Ciphertext
        var combined = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

        // Return URL-Safe Base64 string
        return Convert.ToBase64String(combined)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public int Decode(string encodedId)
    {
        if (!TryDecode(encodedId, out var id))
        {
            throw new ArgumentException("Invalid encrypted ID format.", nameof(encodedId));
        }
        return id;
    }

    public bool TryDecode(string encodedId, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(encodedId))
        {
            return false;
        }

        try
        {
            // Restore standard Base64 padding and characters
            var incoming = encodedId.Replace("-", "+").Replace("_", "/");
            switch (incoming.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            var combined = Convert.FromBase64String(incoming);
            if (combined.Length < 16 + sizeof(int))
            {
                return false;
            }

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16];
            var cipherBytes = new byte[combined.Length - 16];

            Buffer.BlockCopy(combined, 0, iv, 0, 16);
            Buffer.BlockCopy(combined, 16, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            if (plainBytes.Length < sizeof(int))
            {
                return false;
            }

            id = BitConverter.ToInt32(plainBytes, 0);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
