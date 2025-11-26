using Domain.Entities;
using System;
using Xunit;

namespace EmailsP.Tests.Domain
{
    public class UsuarioTests
    {
        [Fact]
        public void Usuario_CanSetAndGetProperties()
        {
            // Arrange
            var id = 1;
            var username = "testuser";
            var passwordHash = "hashedpassword123";
            var email = "test@example.com";
            var rol = "Admin";
            var estaActivo = true;
            var fechaCreacion = DateTime.UtcNow;

            // Act
            var usuario = new Usuario
            {
                Id = id,
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                Rol = rol,
                EstaActivo = estaActivo,
                FechaCreacion = fechaCreacion
            };

            // Assert
            Assert.Equal(id, usuario.Id);
            Assert.Equal(username, usuario.Username);
            Assert.Equal(passwordHash, usuario.PasswordHash);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(rol, usuario.Rol);
            Assert.Equal(estaActivo, usuario.EstaActivo);
            Assert.Equal(fechaCreacion, usuario.FechaCreacion);
        }

        [Fact]
        public void Usuario_DefaultValuesAreSetCorrectly()
        {
            // Arrange & Act
            var usuario = new Usuario();

            // Assert
            Assert.Equal(0, usuario.Id);
            Assert.Equal(string.Empty, usuario.Username);
            Assert.Equal(string.Empty, usuario.PasswordHash);
            Assert.Equal(string.Empty, usuario.Email);
            Assert.Equal("User", usuario.Rol); // Default value
            Assert.True(usuario.EstaActivo); // Default value
            Assert.NotEqual(default(DateTime), usuario.FechaCreacion); // Should be set to UtcNow
        }

        [Fact]
        public void Usuario_FechaCreacion_IsSetToUtcNowOnInstantiation()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var usuario = new Usuario();

            // Assert
            var afterCreation = DateTime.UtcNow;
            Assert.True(usuario.FechaCreacion >= beforeCreation && usuario.FechaCreacion <= afterCreation);
            Assert.True(usuario.FechaCreacion.Kind == DateTimeKind.Utc);
        }
    }
}
