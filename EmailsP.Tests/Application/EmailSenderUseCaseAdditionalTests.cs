using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EmailsP.Tests.Application;

public class EmailSenderUseCaseAdditionalTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly EmailSenderUseCase _emailSenderUseCase;

    public EmailSenderUseCaseAdditionalTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _emailSenderUseCase = new EmailSenderUseCase(_emailServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleRecipients_CallsEmailServiceWithAllRecipients()
    {
        // Arrange
        var recipients = new List<string> { "user1@test.com", "user2@test.com", "user3@test.com" };
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        await _emailSenderUseCase.ExecuteAsync(recipients, subject, body, null);

        // Assert
        _emailServiceMock.Verify(x => x.SendAsync(
            It.Is<IEnumerable<string>>(r => r.Count() == 3 && r.Contains("user1@test.com")),
            subject,
            body,
            null,
            default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyBody_StillCallsEmailService()
    {
        // Arrange
        var recipients = new List<string> { "test@test.com" };
        var subject = "Test Subject";
        string? body = null;

        // Act
        await _emailSenderUseCase.ExecuteAsync(recipients, subject, body, null);

        // Assert
        _emailServiceMock.Verify(x => x.SendAsync(
            It.IsAny<IEnumerable<string>>(),
            subject,
            null,
            null,
            default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PassesTokenToEmailService()
    {
        // Arrange
        var recipients = new List<string> { "test@test.com" };
        var subject = "Test";
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _emailSenderUseCase.ExecuteAsync(recipients, subject, "body", null, token);

        // Assert
        _emailServiceMock.Verify(x => x.SendAsync(
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IEnumerable<IFormFile>>(),
            token), Times.Once);
    }
}
