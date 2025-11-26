using Application.Services;
using Domain.Interfaces;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EmailsP.Tests.Application
{
    public class TextRefactorUseCaseTests
    {
        private readonly Mock<IAIService> _mockAIService;
        private readonly TextRefactorUseCase _textRefactorUseCase;

        public TextRefactorUseCaseTests()
        {
            _mockAIService = new Mock<IAIService>();
            _textRefactorUseCase = new TextRefactorUseCase(_mockAIService.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WithText_CallsAIServiceAndReturnsResult()
        {
            // Arrange
            var rawText = "this is some raw text.";
            var refactoredText = "This is some refactored text.";

            _mockAIService.Setup(s => s.RefactorTextAsync(rawText))
                          .ReturnsAsync(refactoredText);

            // Act
            var result = await _textRefactorUseCase.ExecuteAsync(rawText);

            // Assert
            Assert.Equal(refactoredText, result);
            _mockAIService.Verify(s => s.RefactorTextAsync(rawText), Times.Once);
        }
    }
}
