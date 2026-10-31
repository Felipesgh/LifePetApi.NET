namespace LifePet.Application.Abstractions;

public sealed record AuthUser(string Username, string Password, string Role);

public interface IAuthUserStore
{
    AuthUser? Find(string username);
}
