using AJT.Contracts;
using AJT.Entities;
using AJT.Models;
using AJT.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AJT.Services
{
    internal sealed class HashingService : IHashingService
    {
        private readonly IOptions<AJTOptions> _options;

        public HashingService(IOptions<AJTOptions> options)
        {
            _options = options;
        }

        public string Hash(Token token)
        {
            var tokenJson = JsonConvert.SerializeObject(token);
            var tokenJson64 = Base64UrlEncode(Encoding.UTF8.GetBytes(tokenJson));
            var signature = CreateSignature(tokenJson, _options.Value.Secret);
            return string.Join('.', tokenJson64, signature);
        }

        public string Hash(RefreshToken refreshToken)
        {
            var tokenJson = JsonConvert.SerializeObject(refreshToken);
            var tokenJson64 = Base64UrlEncode(Encoding.UTF8.GetBytes(tokenJson));
            var signature = CreateSignature(tokenJson, _options.Value.Secret);
            return string.Join('.', tokenJson64, signature);
        }

        public bool VerifiyHash(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 2)
                return false;

            string unsignedToken = parts[0];
            string signature = parts[1];
            string expectedSignature = CreateSignature(unsignedToken, _options.Value.Secret);

            return signature == expectedSignature;
        }

        private static string CreateSignature(string unsignedToken, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(unsignedToken);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(messageBytes);
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
