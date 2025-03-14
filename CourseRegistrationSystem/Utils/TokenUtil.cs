using CourseRegistration_Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.IO; 
namespace CourseRegistration_API.Utils
{
	public class TokenUtil
	{ private static readonly string EncryptionKey = "EncryptionKey123!@#$%^&*()" + 
                                                      "ThisNeedsToBe32CharsLong!!";
        private static readonly string SigningKey = "SigningKeyForHMAC!@#$%^&*()123" +
                                                   "ThisMustBeVerySecretAndLong!!!";

        public static string GenerateRefreshToken(User user, Guid specificId)
        {
            try
            {
                // Create a payload with minimal required information
                var payload = $"{user.Id}:{specificId}:{user.Role.Id}:{DateTime.UtcNow.AddDays(30):yyyyMMddHHmmss}";
                
                // Encrypt the payload
                var encryptedPayload = Encrypt(payload);
                
                // Generate signature to verify later
                var signature = GenerateSignature(encryptedPayload);
                
                // Combine encrypted payload and signature into token
                return $"{encryptedPayload}.{signature}";
            }
            catch
            {
                // Fall back to a random token if encryption fails
                var randomToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                return randomToken;
            }
        }

        public static (bool isValid, Guid userId, Guid specificId) ValidateRefreshToken(string refreshToken)
        {
            try
            {
                // Split token into payload and signature
                var parts = refreshToken.Split('.');
                if (parts.Length != 2)
                    return (false, Guid.Empty, Guid.Empty);

                var encryptedPayload = parts[0];
                var signature = parts[1];
                
                // Validate signature
                var expectedSignature = GenerateSignature(encryptedPayload);
                if (signature != expectedSignature)
                    return (false, Guid.Empty, Guid.Empty);

                // Decrypt payload
                var payload = Decrypt(encryptedPayload);
                var segments = payload.Split(':');
                if (segments.Length != 4)
                    return (false, Guid.Empty, Guid.Empty);

                // Extract data
                var userId = Guid.Parse(segments[0]);
                var specificId = Guid.Parse(segments[1]);
                var expiryDate = DateTime.ParseExact(segments[3], "yyyyMMddHHmmss", 
                                                   System.Globalization.CultureInfo.InvariantCulture);
                
                // Check if expired
                if (expiryDate < DateTime.UtcNow)
                    return (false, Guid.Empty, Guid.Empty);
                
                return (true, userId, specificId);
            }
            catch
            {
                return (false, Guid.Empty, Guid.Empty);
            }
        }

        // Encrypt text using AES
        private static string Encrypt(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        // Decrypt text using AES
        private static string Decrypt(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Generate HMAC signature
        private static string GenerateSignature(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SigningKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
