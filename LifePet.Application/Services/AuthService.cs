using LifePet.Application.Abstractions;
using LifePet.Application.DTOs;
using LifePet.Domain.Exceptions;

namespace LifePet.Application.Services;

public class AuthService(IAuthUserStore users, ITokenService tokens) : IAuthService
{
    public LoginResponse Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new BusinessException("Usuário e senha são obrigatórios.");

        var user = users.Find(request.Username);
        if (user is null || !string.Equals(user.Password, request.Password, StringComparison.Ordinal))
            throw new BusinessException("Credenciais inválidas.");

        return new LoginResponse
        {
            AccessToken = tokens.CreateToken(user.Username, user.Role),
            TokenType = "Bearer",
            ExpiresIn = tokens.ExpirationSeconds
        };
    }
}

public interface IAuthService
{
    LoginResponse Login(LoginRequest request);
}
