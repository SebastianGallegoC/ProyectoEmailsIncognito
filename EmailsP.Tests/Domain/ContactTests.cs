using Domain.Entities;
using System;
using Xunit;

namespace EmailsP.Tests.Domain
{
    public class ContactTests
    {
        [Fact]
        public void Contact_CanSetAndGetProperties()
        {
            // Arrange
            var id = 1;
            var usuarioId = 10;
            var name = "John Doe";
            var email = "john.doe@example.com";
            var phoneNumber = "123-456-7890";
            var notes = "Important client";
            var isFavorite = true;
            var isBlocked = false;
            var createdAt = DateTime.UtcNow.AddDays(-5);
            var updatedAt = DateTime.UtcNow.AddDays(-1);

            // Act
            var contact = new Contact
            {
                Id = id,
                UsuarioId = usuarioId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Notes = notes,
                IsFavorite = isFavorite,
                IsBlocked = isBlocked,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(id, contact.Id);
            Assert.Equal(usuarioId, contact.UsuarioId);
            Assert.Equal(name, contact.Name);
            Assert.Equal(email, contact.Email);
            Assert.Equal(phoneNumber, contact.PhoneNumber);
            Assert.Equal(notes, contact.Notes);
            Assert.Equal(isFavorite, contact.IsFavorite);
            Assert.Equal(isBlocked, contact.IsBlocked);
            Assert.Equal(createdAt, contact.CreatedAt);
            Assert.Equal(updatedAt, contact.UpdatedAt);
        }

        [Fact]
        public void Contact_DefaultValuesAreSetCorrectly()
        {
            // Arrange & Act
            var contact = new Contact();

            // Assert
            Assert.Equal(0, contact.Id);
            Assert.Equal(0, contact.UsuarioId);
            Assert.Equal(string.Empty, contact.Name); // Default value from `null!`
            Assert.Equal(string.Empty, contact.Email); // Default value from `null!`
            Assert.Null(contact.PhoneNumber);
            Assert.Null(contact.Notes);
            Assert.False(contact.IsFavorite);
            Assert.False(contact.IsBlocked);
            Assert.NotEqual(default(DateTime), contact.CreatedAt); // Should be set to UtcNow
            Assert.Null(contact.UpdatedAt);
        }

        [Fact]
        public void Contact_CreatedAt_IsSetToUtcNowOnInstantiation()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var contact = new Contact();

            // Assert
            var afterCreation = DateTime.UtcNow;
            Assert.True(contact.CreatedAt >= beforeCreation && contact.CreatedAt <= afterCreation);
            Assert.True(contact.CreatedAt.Kind == DateTimeKind.Utc);
        }
    }
}
