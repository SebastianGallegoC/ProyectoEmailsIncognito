using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;

namespace EmailsP.Tests.IntegrationTests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedAndToken()
    {
        // Arrange
        var uniqueEmail = $"testuser_{Guid.NewGuid()}@example.com";
        var request = new RegisterRequest
        {
            Email = uniqueEmail,
            Password = "Password123",
            Username = $"testuser_{Guid.NewGuid().ToString().Substring(0, 4)}" // Unique username with at least 3 chars
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        if (response.StatusCode != HttpStatusCode.Created)
        {
            Assert.Fail($"Expected status code Created but received {response.StatusCode}. Response: {responseContent}");
        }

        Assert.False(string.IsNullOrWhiteSpace(responseContent));

        // Optional: Verify that the response contains a token
        using var jsonDoc = JsonDocument.Parse(responseContent);
        Assert.True(jsonDoc.RootElement.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        var uniqueEmail = $"testuser_{Guid.NewGuid()}@example.com";
        var uniqueUsername = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}";
        var password = "Password123";

        var registerRequest = new RegisterRequest
        {
            Email = uniqueEmail,
            Password = password,
            Username = uniqueUsername
        };

        // Register a new user
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest
        {
            Username = uniqueUsername,
            Password = password
        };

        // Act
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var responseContent = await loginResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(responseContent));

        using var jsonDoc = JsonDocument.Parse(responseContent);
        Assert.True(jsonDoc.RootElement.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "nonexistentuser",
            Password = "wrongpassword"
        };

        // Act
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var uniqueEmail1 = $"testuser1_{Guid.NewGuid()}@example.com";
        var uniqueEmail2 = $"testuser2_{Guid.NewGuid()}@example.com";
        var username = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}";
        var password = "Password123";

        var request1 = new RegisterRequest
        {
            Email = uniqueEmail1,
            Password = password,
            Username = username
        };

        var request2 = new RegisterRequest
        {
            Email = uniqueEmail2,
            Password = password,
            Username = username
        };

        // Register the first user
        var response1 = await _client.PostAsJsonAsync("/api/auth/register", request1);
        response1.EnsureSuccessStatusCode();

        // Act
        // Try to register the second user with the same username
        var response2 = await _client.PostAsJsonAsync("/api/auth/register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = $"testuser_{Guid.NewGuid()}@example.com";
        var username1 = $"testuser1_{Guid.NewGuid().ToString().Substring(0, 8)}";
        var username2 = $"testuser2_{Guid.NewGuid().ToString().Substring(0, 8)}";
        var password = "Password123";

        var request1 = new RegisterRequest
        {
            Email = email,
            Password = password,
            Username = username1
        };

        var request2 = new RegisterRequest
        {
            Email = email,
            Password = password,
            Username = username2
        };

        // Register the first user
        var response1 = await _client.PostAsJsonAsync("/api/auth/register", request1);
        response1.EnsureSuccessStatusCode();

        // Act
        // Try to register the second user with the same email
        var response2 = await _client.PostAsJsonAsync("/api/auth/register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidUsername_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Password123",
            Username = "a" // Invalid username
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "123", // Invalid password
            Username = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email", // Invalid email
            Password = "Password123",
            Username = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
