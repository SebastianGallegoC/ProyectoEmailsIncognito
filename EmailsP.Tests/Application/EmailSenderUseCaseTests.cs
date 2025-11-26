using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EmailsP.Tests.Application
{
    public class EmailSenderUseCaseTests
    {
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly EmailSenderUseCase _emailSenderUseCase;

        public EmailSenderUseCaseTests()
        {
            _mockEmailService = new Mock<IEmailService>();
            _emailSenderUseCase = new EmailSenderUseCase(_mockEmailService.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidParameters_CallsEmailServiceSendAsync()
        {
            // Arrange
            var to = new[] { "test@example.com" };
            var subject = "Test Subject";
            var body = "<p>Test Body</p>";
            
            // Mock IFormFile
            var mockAttachment = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            mockAttachment.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockAttachment.Setup(_ => _.FileName).Returns(fileName);
            mockAttachment.Setup(_ => _.Length).Returns(ms.Length);

            var attachments = new[] { mockAttachment.Object };
            var cancellationToken = new CancellationToken();

            _mockEmailService.Setup(s => s.SendAsync(to, subject, body, attachments, cancellationToken))
                             .Returns(Task.CompletedTask);

            // Act
            await _emailSenderUseCase.ExecuteAsync(to, subject, body, attachments, cancellationToken);

            // Assert
            _mockEmailService.Verify(s => s.SendAsync(to, subject, body, attachments, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullAttachments_CallsEmailServiceSendAsync()
        {
            // Arrange
            var to = new[] { "test@example.com" };
            var subject = "Test Subject";
            var body = "<p>Test Body</p>";
            IEnumerable<IFormFile>? attachments = null;
            var cancellationToken = new CancellationToken();

            _mockEmailService.Setup(s => s.SendAsync(to, subject, body, attachments, cancellationToken))
                             .Returns(Task.CompletedTask);

            // Act
            await _emailSenderUseCase.ExecuteAsync(to, subject, body, attachments, cancellationToken);

            // Assert
            _mockEmailService.Verify(s => s.SendAsync(to, subject, body, attachments, cancellationToken), Times.Once);
        }
    }
}
