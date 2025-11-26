using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EmailsP.Tests.Application
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _mockRepo;
        private readonly ContactService _contactService;
        private const int TestUserId = 1;

        public ContactServiceTests()
        {
            _mockRepo = new Mock<IContactRepository>();
            _contactService = new ContactService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsContactResponse()
        {
            // Arrange
            var request = new CreateContactRequest { Name = "Test", Email = "test@example.com" };
            _mockRepo.Setup(r => r.EmailExistsAsync(TestUserId, request.Email, null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Contact c, CancellationToken ct) => {
                         c.Id = 1;
                         return c;
                     });

            // Act
            var result = await _contactService.CreateAsync(TestUserId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CreateContactRequest { Name = "Test", Email = "exists@example.com" };
            _mockRepo.Setup(r => r.EmailExistsAsync(TestUserId, request.Email, null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _contactService.CreateAsync(TestUserId, request));
        }

        [Fact]
        public async Task GetAsync_WhenContactExists_ReturnsContactResponse()
        {
            // Arrange
            var contact = new Contact { Id = 1, UsuarioId = TestUserId, Name = "Test" };
            _mockRepo.Setup(r => r.GetByIdAsync(1, TestUserId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(contact);
            
            // Act
            var result = await _contactService.GetAsync(1, TestUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contact.Id, result.Id);
        }

        [Fact]
        public async Task GetAsync_WhenContactDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Contact?)null);

            // Act
            var result = await _contactService.GetAsync(99, TestUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenContactExists_ReturnsUpdatedContact()
        {
            // Arrange
            var contactId = 1;
            var existingContact = new Contact { Id = contactId, UsuarioId = TestUserId, Name = "Old Name", Email = "old@example.com" };
            var updateRequest = new UpdateContactRequest { Name = "New Name", Email = "new@example.com" };

            _mockRepo.Setup(r => r.GetByIdAsync(contactId, TestUserId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(existingContact);
            _mockRepo.Setup(r => r.EmailExistsAsync(TestUserId, updateRequest.Email, contactId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);
            
            // Act
            var result = await _contactService.UpdateAsync(contactId, TestUserId, updateRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateRequest.Name, result.Name);
            Assert.Equal(updateRequest.Email, result.Email);
        }

        [Fact]
        public async Task UpdateAsync_WhenContactDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var updateRequest = new UpdateContactRequest { Name = "Any" };
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Contact?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _contactService.UpdateAsync(99, TestUserId, updateRequest));
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDelete()
        {
            // Arrange
            var contactId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(contactId, TestUserId, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            await _contactService.DeleteAsync(contactId, TestUserId);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(contactId, TestUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_WithNoQuery_ReturnsAllContactsForUser()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { Id = 1, UsuarioId = TestUserId, Name = "Alice", Email = "alice@example.com" },
                new Contact { Id = 2, UsuarioId = TestUserId, Name = "Bob", Email = "bob@example.com" }
            };
            _mockRepo.Setup(r => r.SearchAsync(TestUserId, null, 1, 20, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((contacts, 2));

            // Act
            var result = await _contactService.SearchAsync(TestUserId, null, 1, 20);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("Alice", result.Items.First().Name);
        }

        [Fact]
        public async Task SearchAsync_WithQuery_ReturnsFilteredContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { Id = 1, UsuarioId = TestUserId, Name = "Alice", Email = "alice@example.com" }
            };
            _mockRepo.Setup(r => r.SearchAsync(TestUserId, "Alice", 1, 20, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((contacts, 1));

            // Act
            var result = await _contactService.SearchAsync(TestUserId, "Alice", 1, 20);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Total);
            Assert.Single(result.Items);
            Assert.Equal("Alice", result.Items.First().Name);
        }

        [Fact]
        public async Task SearchAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var allContacts = new List<Contact>
            {
                new Contact { Id = 1, UsuarioId = TestUserId, Name = "Alice", Email = "alice@example.com" },
                new Contact { Id = 2, UsuarioId = TestUserId, Name = "Bob", Email = "bob@example.com" },
                new Contact { Id = 3, UsuarioId = TestUserId, Name = "Charlie", Email = "charlie@example.com" }
            };
            
            // Mock for page 1, pageSize 2
            _mockRepo.Setup(r => r.SearchAsync(TestUserId, null, 1, 2, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((allContacts.Take(2).ToList(), 3));

            // Mock for page 2, pageSize 2
            _mockRepo.Setup(r => r.SearchAsync(TestUserId, null, 2, 2, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((allContacts.Skip(2).Take(1).ToList(), 3));

            // Act - Page 1
            var resultPage1 = await _contactService.SearchAsync(TestUserId, null, 1, 2);

            // Assert - Page 1
            Assert.NotNull(resultPage1);
            Assert.Equal(3, resultPage1.Total);
            Assert.Equal(2, resultPage1.Items.Count());
            Assert.Equal("Alice", resultPage1.Items.First().Name);
            Assert.Equal("Bob", resultPage1.Items.Last().Name);

            // Act - Page 2
            var resultPage2 = await _contactService.SearchAsync(TestUserId, null, 2, 2);

            // Assert - Page 2
            Assert.NotNull(resultPage2);
            Assert.Equal(3, resultPage2.Total);
            Assert.Single(resultPage2.Items);
            Assert.Equal("Charlie", resultPage2.Items.First().Name);
        }
    }
}
