using Domain.Entities;

namespace EmailsP.Tests.Domain;

public class UsuarioValidationTests
{
    [Fact]
    public void Usuario_WithValidEmail_IsValid()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        // Assert
        Assert.Contains("@", usuario.Email);
        Assert.Contains(".", usuario.Email);
    }

    [Fact]
    public void Usuario_DefaultRole_IsUser()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        // Assert
        Assert.Equal("User", usuario.Rol);
    }

    [Fact]
    public void Usuario_CanSetAdminRole()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = "hashedpassword",
            Rol = "Admin"
        };

        // Assert
        Assert.Equal("Admin", usuario.Rol);
    }
}
