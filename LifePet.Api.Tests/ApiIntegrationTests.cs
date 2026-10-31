using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LifePet.Api.Tests;

public sealed class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_live_status()
    {
        using var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();

        using var content = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.Equal("Healthy", content.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Swagger_document_describes_api_routes()
    {
        using var response = await _client.GetAsync("/swagger/v1/swagger.json");
        var document = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, document);
        using var content = JsonDocument.Parse(document);
        Assert.True(content.RootElement.GetProperty("paths").TryGetProperty("/api/Pets", out _));
        Assert.True(
            content.RootElement.GetProperty("components").GetProperty("securitySchemes").TryGetProperty("Bearer", out _),
            content.RootElement.ToString());

        var paths = content.RootElement.GetProperty("paths");
        Assert.NotEmpty(paths.GetProperty("/api/Pets").GetProperty("get").GetProperty("security").EnumerateArray());
        Assert.False(paths.GetProperty("/api/auth/login").GetProperty("post").TryGetProperty("security", out _));
    }

    [Fact]
    public async Task Protected_endpoints_require_authentication()
    {
        using var response = await _client.GetAsync("/api/tutores");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
