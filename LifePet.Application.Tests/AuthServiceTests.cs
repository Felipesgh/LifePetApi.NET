using LifePet.Application.Abstractions;
using LifePet.Application.DTOs;
using LifePet.Application.Services;
using LifePet.Domain.Exceptions;

namespace LifePet.Application.Tests;

public class AuthServiceTests
{
    private sealed class FakeUsers : IAuthUserStore
    {
        public AuthUser? Find(string username)
            => username == "admin" ? new AuthUser("admin", "Admin@123", "Admin") : null;
    }

    private sealed class FakeTokens : ITokenService
    {
        public int ExpirationSeconds => 3600;
        public string CreateToken(string username, string role) => $"token-{username}-{role}";
    }

    [Fact]
    public void Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        // Arrange
        var sut = new AuthService(new FakeUsers(), new FakeTokens());

        // Act
        var response = sut.Login(new LoginRequest { Username = "admin", Password = "Admin@123" });

        // Assert
        Assert.Equal("token-admin-Admin", response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal(3600, response.ExpiresIn);
    }

    [Fact]
    public void Login_DeveFalhar_QuandoSenhaInvalida()
    {
        // Arrange
        var sut = new AuthService(new FakeUsers(), new FakeTokens());

        // Act
        var act = () => sut.Login(new LoginRequest { Username = "admin", Password = "errada" });

        // Assert
        Assert.Throws<BusinessException>(act);
    }
}
