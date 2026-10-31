using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LifePet.Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LifePet.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "LifePetApi";
    public string Audience { get; set; } = "LifePetApi";
    public int ExpirationMinutes { get; set; } = 60;
}

public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public int ExpirationSeconds => options.Value.ExpirationMinutes * 60;

    public string CreateToken(string username, string role)
    {
        var jwt = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            jwt.Issuer,
            jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
