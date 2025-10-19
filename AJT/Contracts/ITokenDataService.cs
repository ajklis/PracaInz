namespace AJT.Contracts
{
    internal interface ITokenDataService
    {
        public void RegisterAction(Func<Guid, IServiceProvider, Task<object>> func);
        public Task<object?> GetCustomTokenData(Guid userId);
    }
}
