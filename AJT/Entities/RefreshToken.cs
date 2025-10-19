namespace AJT.Entities
{
    public sealed class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
