using BCrypt.Net;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EmailsP.Tests.Application
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockUserRepo;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockUserRepo = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_mockUserRepo.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesUserWithHashedPasswordAndDefaultRole()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "newuser",
                Password = "Password123",
                Email = "newuser@example.com"
            };

            Usuario? capturedUser = null;

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(request.Username))
                         .ReturnsAsync((Usuario?)null); // El usuario no existe

            _mockUserRepo.Setup(repo => repo.CreateAsync(It.IsAny<Usuario>()))
                         .Callback<Usuario>(user => capturedUser = user) // Capturar el usuario
                         .ReturnsAsync((Usuario user) => {
                             user.Id = 1; // Simular que la BD asigna un ID
                             return user;
                         });

            // Act
            var result = await _usuarioService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Username, result.Username);
            Assert.Equal(request.Email, result.Email);
            Assert.NotEqual(0, result.Id);

            _mockUserRepo.Verify(repo => repo.CreateAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.NotNull(capturedUser);
            Assert.Equal("User", capturedUser.Rol);
            Assert.True(capturedUser.EstaActivo);
            Assert.NotEmpty(capturedUser.PasswordHash);
            Assert.NotEqual(request.Password, capturedUser.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, capturedUser.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "existinguser",
                Password = "Password123",
                Email = "newuser@example.com"
            };

            var existingUser = new Usuario { Username = "existinguser" };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(request.Username))
                         .ReturnsAsync(existingUser); // El usuario ya existe

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.RegisterAsync(request));
            
            _mockUserRepo.Verify(repo => repo.CreateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "newuser_unique",
                Password = "Password123",
                Email = "existing@example.com"
            };

            var existingUserWithEmail = new Usuario { Email = "existing@example.com" };

            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(request.Username))
                         .ReturnsAsync((Usuario?)null); // Username is unique

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(request.Email))
                         .ReturnsAsync(existingUserWithEmail); // Email already exists

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.RegisterAsync(request));

            _mockUserRepo.Verify(repo => repo.CreateAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
