using AJT.Entities;
using AJT.Models;

namespace AJT.Contracts
{
    internal interface IHashingService
    {
        string Hash(Token token);
        string Hash(RefreshToken refreshToken);
        string DecodePayload(string hashedToken);
        bool VerifiyHash(string hashed);
    }
}
