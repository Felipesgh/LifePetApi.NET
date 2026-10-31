using LifePet.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace LifePet.Infrastructure.Auth;

public sealed class ConfigAuthUserStore(IConfiguration configuration) : IAuthUserStore
{
    public AuthUser? Find(string username)
    {
        var users = configuration.GetSection("Auth:Users").Get<List<AuthUser>>() ?? [];
        return users.FirstOrDefault(user => string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
    }
}
