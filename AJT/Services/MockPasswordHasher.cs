using AJT.Contracts;
using System.Text;

namespace AJT.Services
{
    internal sealed class MockPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
    }
}
