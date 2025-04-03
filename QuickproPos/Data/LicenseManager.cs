using System.Security.Cryptography;
using System.Text;

namespace QuickproPos.Data
{
    public class LicenseManager
    {
        public static string GenerateLicenseKey(string userName, string email, string machineId)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Create raw data for license key generation
                string rawData = userName + email + machineId + DateTime.Now.ToString();

                // Hash the data using SHA256
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert to string representation
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("X2")); // X2 formats as hexadecimal
                }

                // Return the first 20 characters as the license key
                return builder.ToString().Substring(0, 20);
            }
        }
    }
}
