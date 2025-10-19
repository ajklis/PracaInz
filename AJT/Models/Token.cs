namespace AJT.Models
{
    public sealed class Token
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public object Data { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
