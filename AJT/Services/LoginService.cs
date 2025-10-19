using AJT.Contracts;
using AJT.Entities;
using AJT.Models;
using AJT.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AJT.Services
{
    internal sealed class LoginService : ILoginService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<AJTOptions> _options;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRoleService _roleService;
        private readonly IHashingService _hashingService;
        private readonly ILogger<LoginService> _logger;

        public LoginService(IServiceScopeFactory scopeFactory, IOptions<AJTOptions> options, IPasswordHasher passwordHasher, IRoleService roleService, IHashingService hashingService, ILogger<LoginService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options;
            _passwordHasher = passwordHasher;
            _roleService = roleService;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<CombinedToken?> Login(string login, string password)
        {
            var hashedPassword = _passwordHasher.HashPassword(password);

            using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
            var user = await userRepo.GetUserByLogin(login);

            if (user is null || user.HashedPassword != hashedPassword)
                return null;

            return await CreateCombinedTokenForUser(scope, user);
        }

        public async Task<CombinedToken?> Refresh(string refreshTokenString)
        {
            if (!_hashingService.VerifiyHash(refreshTokenString))
                return null;

            RefreshToken refreshToken = null;
            try
            {
                var json = _hashingService.DecodePayload(refreshTokenString);
                refreshToken = JsonConvert.DeserializeObject<RefreshToken>(json);
                if (refreshToken is null)
                    return null;
            }
            catch
            {
                return null;
            }

            if (refreshToken.ExpirationDate < DateTime.Now)
                return null;

            using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
            var user = await userRepo.GetUserById(refreshToken.UserId);

            if (user is null)
                return null;

            return await CreateCombinedTokenForUser(scope, user);
        }

        public async Task<bool> Register(string username, string email, string password)
        {
            using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

            var existingUserUsername = await userRepo.GetUserByLogin(username);
            var existingUserEmail = await userRepo.GetUserByLogin(email);

            if (existingUserUsername is not null || existingUserEmail is not null)
                return false;

            var user = new User
            {
                Username = username,
                Email = email,
                HashedPassword = _passwordHasher.HashPassword(password)
            };

            try
            {
                await userRepo.AddUser(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<CombinedToken?> CreateCombinedTokenForUser(IServiceScope scope, User user)
        {
            var userRoleRepo = scope.ServiceProvider.GetRequiredService<IUserRoleRepo>();
            var roles = await userRoleRepo.GetUserRoles(user);

            var token = new Token
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ExpirationDate = DateTime.Now.Add(_options.Value.TokenExpirationTime)
            };

            if (roles is not null)
                token.UserRoles = await _roleService.EncodeRoles(roles.Select(x => x.RoleCode).ToList());

            var refreshTokenRepo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepo>();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                ExpirationDate = DateTime.Now.Add(_options.Value.RefreshTokenExpirationTime)
            };
            await refreshTokenRepo.AddRefreshToken(refreshToken);

            return new CombinedToken
            {
                Token = _hashingService.Hash(token),
                RefreshToken = _hashingService.Hash(refreshToken)
            };
        }
    }
}
