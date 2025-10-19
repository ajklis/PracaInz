namespace AJT.Contracts
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
    }
}
