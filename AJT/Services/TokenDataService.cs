using AJT.Contracts;

namespace AJT.Services
{
    internal sealed class TokenDataService : ITokenDataService
    {
        private Func<Guid, IServiceProvider, Task<object>>? _func;
        private readonly IServiceProvider _serviceProvider;

        public static Func<Guid, IServiceProvider, Task<object>>? InitFunc { get; set; }

        public TokenDataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _func = InitFunc;
        }

        public async Task<object?> GetCustomTokenData(Guid userId)
        {
            if (_func is null)
                return null;

            return await _func(userId, _serviceProvider);
        }

        public void RegisterAction(Func<Guid, IServiceProvider, Task<object>> func)
        {
            _func = func;
        }
    }
}
