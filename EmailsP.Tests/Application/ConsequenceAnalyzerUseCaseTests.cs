using Application.DTOs.AI;
using Application.Services;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;



namespace EmailsP.Tests.Application
{
    public class ConsequenceAnalyzerUseCaseTests
    {
        private readonly Mock<IConsequenceAnalyzerService> _mockAnalyzerService;
        private readonly Mock<ILogger<ConsequenceAnalyzerUseCase>> _mockLogger;
        private readonly ConsequenceAnalyzerUseCase _useCase;

        public ConsequenceAnalyzerUseCaseTests()
        {
            _mockAnalyzerService = new Mock<IConsequenceAnalyzerService>();
            _mockLogger = new Mock<ILogger<ConsequenceAnalyzerUseCase>>();
            _useCase = new ConsequenceAnalyzerUseCase(_mockAnalyzerService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnalysisSucceeds_ReturnsMappedDto()
        {
            // Arrange
            var text = "test message";
            var context = "work";
            var country = "US";
            var analysisResult = new ConsequenceAnalysis
            {
                Overall = new OverallAssessment { Summary = "Test Summary" },
                LegalRisk = new LegalRiskAnalysis { Level = RiskLevel.Low }
            };

            _mockAnalyzerService.Setup(s => s.AnalyzeAsync(text, context, country))
                                .ReturnsAsync(analysisResult);

            // Act
            var result = await _useCase.ExecuteAsync(text, context, country);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisResult.Overall.Summary, result.Overall.Summary);
            Assert.Equal(analysisResult.LegalRisk.Level.ToString(), result.LegalRisk.Level);
            _mockAnalyzerService.Verify(s => s.AnalyzeAsync(text, context, country), Times.Once);

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Analizando consecuencias")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenAnalysisFails_ThrowsAndLogsError()
        {
            // Arrange
            var text = "test message";
            var exception = new InvalidOperationException("AI service failed");

            _mockAnalyzerService.Setup(s => s.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(text));

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al analizar consecuencias")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
