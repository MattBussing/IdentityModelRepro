using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace BufferRepro
{
    internal class Program
    {
        static void Main(string path)
        {
            if (path == null)
            {
                throw new Exception($"you need to set {nameof(path)}");
            }

            if (File.Exists(path))
            {
                string fileContent = File.ReadAllText(path);

                if (Path.GetExtension(path).ToLower() == ".xml")
                {
                    TestKey(fileContent, false);
                    TestKey(fileContent, true);
                }
            }
            else
            {
                throw new Exception($"File {path} does not exist.");
            }
        }

        private static void TestKey(string fileContent, bool useBadVersion)
        {
            RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(fileContent);

            RsaSecurityKey key = useBadVersion ? new(rsa.ExportParameters(true)) : new(rsa);
            SigningCredentials signingCredentials = new(key, SecurityAlgorithms.RsaSha256);
            DateTime now = DateTime.Now;
            JwtSecurityTokenHandler handler = new();
            string encodedJwt = handler.CreateEncodedJwt(
                "bob",
                "alice",
                new ClaimsIdentity(),
                now,
                now.AddMinutes(60),
                now,
                signingCredentials
            );

            if (string.IsNullOrEmpty(encodedJwt))
            {
                throw new Exception("jwt is empty");
            }
            else
            {
                Console.WriteLine(encodedJwt);
            }
        }
    }
}
