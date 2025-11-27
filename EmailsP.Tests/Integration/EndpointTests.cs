using Application.DTOs;
using Application.DTOs.AI;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace EmailsP.Tests.Integration
{
    /// <summary>
    /// Tests de integración simplificados que verifican que todos los endpoints del sistema existen y responden correctamente.
    /// Estos tests verifican que las rutas estén configuradas y los controladores respondan.
    /// Si estos tests fallan, el CI/CD no debe permitir el despliegue.
    /// </summary>
    public class EndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public EndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        #region Auth Endpoints Tests

        [Fact]
        public async Task POST_AuthLogin_EndpointExists_Returns401Or200()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            // Assert - Endpoint should exist (not 404)
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task POST_AuthRegister_EndpointExists_AcceptsRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = $"testuser_{Guid.NewGuid():N}",
                Email = $"test_{Guid.NewGuid():N}@test.com",
                Password = "Test123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

            // Assert - Endpoint should exist (not 404)
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Contacts Endpoints Tests

        [Fact]
        public async Task POST_Contacts_WithoutAuth_Returns401()
        {
            // Arrange
            var createRequest = new CreateContactRequest
            {
                Name = "Test Contact",
                Email = "test@test.com"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Contacts", createRequest);

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_Contacts_WithoutAuth_Returns401()
        {
            // Act
            var response = await _client.GetAsync("/api/Contacts");

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_ContactsById_WithoutAuth_Returns401()
        {
            // Act
            var response = await _client.GetAsync("/api/Contacts/1");

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PUT_Contacts_WithoutAuth_Returns401()
        {
            // Arrange
            var updateRequest = new UpdateContactRequest
            {
                Name = "Updated Name",
                Email = "updated@test.com"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/Contacts/1", updateRequest);

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_Contacts_WithoutAuth_Returns401()
        {
            // Act
            var response = await _client.DeleteAsync("/api/Contacts/1");

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region AI Endpoints Tests

        [Fact]
        public async Task POST_AIRefactor_WithoutAuth_Returns401()
        {
            // Arrange
            var refactorRequest = new RefactorRequest
            {
                Text = "This is a test text"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/AI/refactor", refactorRequest);

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_AIAnalyzeConsequences_WithoutAuth_Returns401()
        {
            // Arrange
            var analyzeRequest = new AnalyzeConsequencesRequest
            {
                Text = "This is a test text for analysis"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/AI/analyze-consequences", analyzeRequest);

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Email Endpoints Tests

        [Fact]
        public async Task POST_EmailSendSimple_WithoutAuth_Returns401()
        {
            // Arrange
            var emailRequest = new EmailRequest
            {
                To = new List<string> { "test@test.com" },
                Subject = "Test",
                Body = "Test body"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Email/SendSimple", emailRequest);

            // Assert - Should require authentication
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Route Existence Tests

        [Fact]
        public async Task POST_AuthLogin_RouteExists()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "test", Password = "test" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            // Assert
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task POST_AuthRegister_RouteExists()
        {
            // Arrange
            var registerRequest = new RegisterRequest 
            { 
                Username = "test", 
                Email = "test@test.com", 
                Password = "test" 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

            // Assert
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task GET_Contacts_RouteExists()
        {
            // Act
            var response = await _client.GetAsync("/api/Contacts");

            // Assert - 401 means route exists but needs auth
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.OK,
                $"Expected 401 or 200, got {response.StatusCode}");
        }

        [Fact]
        public async Task POST_AIRefactor_RouteExists()
        {
            // Arrange
            var refactorRequest = new RefactorRequest { Text = "test" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/AI/refactor", refactorRequest);

            // Assert - 401 means route exists but needs auth
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected 401 or 400, got {response.StatusCode}");
        }

        [Fact]
        public async Task POST_AIAnalyzeConsequences_RouteExists()
        {
            // Arrange
            var analyzeRequest = new AnalyzeConsequencesRequest { Text = "test text" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/AI/analyze-consequences", analyzeRequest);

            // Assert - 401 means route exists but needs auth
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected 401 or 400, got {response.StatusCode}");
        }

        [Fact]
        public async Task POST_EmailSendSimple_RouteExists()
        {
            // Arrange
            var emailRequest = new EmailRequest 
            { 
                To = new List<string> { "test@test.com" },
                Subject = "test", 
                Body = "test" 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Email/SendSimple", emailRequest);

            // Assert - 401 means route exists but needs auth
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected 401 or 400, got {response.StatusCode}");
        }

        #endregion
    }
}
