using AJT.Contracts;
using AJT.Entities;
using AJT.Models;
using AJT.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;

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
            var signature = CreateSignature(tokenJson64, _options.Value.Secret);
            return string.Join('.', tokenJson64, signature);
        }

        public string Hash(RefreshToken refreshToken)
        {
            var tokenJson = JsonConvert.SerializeObject(refreshToken);
            var tokenJson64 = Base64UrlEncode(Encoding.UTF8.GetBytes(tokenJson));
            var signature = CreateSignature(tokenJson64, _options.Value.Secret);
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

        public string DecodePayload(string hashedToken)
        {
            var parts = hashedToken.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid AJT");

            var payload = parts[0];
            var bytes = Base64UrlDecode(payload);
            return Encoding.UTF8.GetString(bytes);
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

        private static byte[] Base64UrlDecode(string input)
        {
            string padded = input.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }


    }
}
