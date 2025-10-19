namespace AJT.Options
{
    public sealed class AJTOptions
    {
        public string ConnectionString { get; set; }
        public TimeSpan TokenExpirationTime { get; set; }
        public TimeSpan RefreshTokenExpirationTime { get; set; }
        public List<string> Roles { get; set; }
        public bool DetectRolesFromAssembly { get; set; }
        public string Secret { get; set; }

    }
}
