using System;
using System.Security.Cryptography;
using System.Text;

namespace CHOY.App_Code.Auth.Jwt
{
    public class JwsAlgorithm
    {
        public string Hash(string hashAlg, string header, string payload, string secretKey)
        {
            string result;
            string content = $"{header}.{payload}";

            switch (hashAlg.ToUpper())
            {
                case "HS256":
                    result = HMAC_SHA256(content, secretKey);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }
        public string HMAC_SHA256(string msg, string secretKey)
        {
            byte[] bytes_secretKey = Encoding.UTF8.GetBytes(secretKey);
            byte[] bytes_msg = Encoding.UTF8.GetBytes(msg);

            using (HMACSHA256 hmacSHA256 = new HMACSHA256(bytes_secretKey))
            {
                byte[] hashMessage = hmacSHA256.ComputeHash(bytes_msg);
                return Convert.ToBase64String(hashMessage).Replace("=", "").Replace('+', '-').Replace('/', '_');
            }
        }
    }
}