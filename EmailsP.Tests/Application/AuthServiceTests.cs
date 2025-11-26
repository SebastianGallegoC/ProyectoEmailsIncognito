using BCrypt.Net;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EmailsP.Tests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockUserRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUsuarioRepository>();
            _mockConfig = new Mock<IConfiguration>();

            // Mock configuration for JWT using the indexer
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("some-super-secret-key-that-is-long-enough-for-hs256");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_mockConfig.Object, _mockUserRepo.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var username = "testuser";
            var password = "Password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new Usuario
            {
                Id = 1,
                Username = username,
                PasswordHash = passwordHash,
                Email = "test@example.com",
                Rol = "User",
                EstaActivo = true
            };

            var loginRequest = new LoginRequest { Username = username, Password = password };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username))
                         .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoginResponse>(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }
        [Fact]
        public async Task AuthenticateAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "nonexistent", Password = "password" };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync("nonexistent"))
                         .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInactiveUser_ReturnsNull()
        {
            // Arrange
            var username = "inactiveuser";
            var user = new Usuario { Username = username, EstaActivo = false };
            var loginRequest = new LoginRequest { Username = username, Password = "password" };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username))
                         .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithIncorrectPassword_ReturnsNull()
        {
            // Arrange
            var username = "testuser";
            var password = "Password123";
            var incorrectPassword = "WrongPassword";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new Usuario { Username = username, PasswordHash = passwordHash, EstaActivo = true };
            var loginRequest = new LoginRequest { Username = username, Password = incorrectPassword };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username))
                         .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithMissingJwtKey_ThrowsException()
        {
            // Arrange
            var username = "testuser";
            var password = "Password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new Usuario
            {
                Id = 1,
                Username = username,
                PasswordHash = passwordHash,
                Email = "test@example.com",
                Rol = "User",
                EstaActivo = true
            };

            var loginRequest = new LoginRequest { Username = username, Password = password };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username))
                         .ReturnsAsync(user);

            // Create a new mock config where Jwt:Key is null or empty
            var mockConfigMissingKey = new Mock<IConfiguration>();
            mockConfigMissingKey.Setup(c => c["Jwt:Key"]).Returns((string?)null); // Simulate missing key
            mockConfigMissingKey.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfigMissingKey.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            var authServiceMissingKey = new AuthService(mockConfigMissingKey.Object, _mockUserRepo.Object);

            // Act & Assert
            // Expecting an ArgumentNullException or similar due to null key in Encoding.UTF8.GetBytes
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await authServiceMissingKey.AuthenticateAsync(loginRequest));
        }
    }
}
