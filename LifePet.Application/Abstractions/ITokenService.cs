namespace LifePet.Application.Abstractions;

public interface ITokenService
{
    string CreateToken(string username, string role);
    int ExpirationSeconds { get; }
}
