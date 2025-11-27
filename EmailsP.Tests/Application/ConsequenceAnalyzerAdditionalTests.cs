using Application.Services;
using Application.DTOs.AI;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmailsP.Tests.Application;

public class ConsequenceAnalyzerAdditionalTests
{
    private readonly Mock<IConsequenceAnalyzerService> _analyzerServiceMock;
    private readonly Mock<ILogger<ConsequenceAnalyzerUseCase>> _loggerMock;
    private ConsequenceAnalyzerUseCase _useCase;

    public ConsequenceAnalyzerAdditionalTests()
    {
        _analyzerServiceMock = new Mock<IConsequenceAnalyzerService>();
        _loggerMock = new Mock<ILogger<ConsequenceAnalyzerUseCase>>();
        _useCase = new ConsequenceAnalyzerUseCase(_analyzerServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithDifferentCountry_PassesCountryToService()
    {
        // Arrange
        var text = "Test message";
        var context = "workplace";
        var country = "US";
        var analysis = CreateMockAnalysis();

        _analyzerServiceMock
            .Setup(x => x.AnalyzeAsync(text, context, country))
            .ReturnsAsync(analysis);

        // Act
        await _useCase.ExecuteAsync(text, context, country);

        // Assert
        _analyzerServiceMock.Verify(x => x.AnalyzeAsync(text, context, country), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_LogsInformationBeforeAnalysis()
    {
        // Arrange
        var text = "Test message";
        var analysis = CreateMockAnalysis();

        _analyzerServiceMock
            .Setup(x => x.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(analysis);

        // Act
        await _useCase.ExecuteAsync(text);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Analizando consecuencias")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDefaultParameters_UsesWorkplaceAndCO()
    {
        // Arrange
        var text = "Test message";
        var analysis = CreateMockAnalysis();

        _analyzerServiceMock
            .Setup(x => x.AnalyzeAsync(text, "workplace", "CO"))
            .ReturnsAsync(analysis);

        // Act
        await _useCase.ExecuteAsync(text);

        // Assert
        _analyzerServiceMock.Verify(x => x.AnalyzeAsync(text, "workplace", "CO"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var text = "Test message";
        var exception = new Exception("Service error");

        _analyzerServiceMock
            .Setup(x => x.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _useCase.ExecuteAsync(text));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al analizar consecuencias")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private ConsequenceAnalysis CreateMockAnalysis()
    {
        return new ConsequenceAnalysis
        {
            LegalRisk = new LegalRiskAnalysis
            {
                Level = RiskLevel.Low,
                Description = "Test",
                PotentialIssues = new List<string>(),
                LegalReferences = new List<string>(),
                PracticalReality = "Test"
            },
            EmotionalImpact = new EmotionalImpactAnalysis
            {
                Level = RiskLevel.Low,
                Description = "Test",
                DetectedTone = "neutral",
                TriggerWords = new List<string>(),
                CulturalContext = "Test"
            },
            Effectiveness = new EffectivenessAnalysis
            {
                ProbabilityOfAction = 50,
                Reasoning = "Test",
                MissingElements = new List<string>(),
                StrengthPoints = new List<string>(),
                LocalRecommendations = "Test"
            },
            Backlash = new BacklashAnalysis
            {
                Level = RiskLevel.Low,
                PotentialConsequences = new List<string>(),
                MitigationAdvice = "Test",
                LocalProtections = "Test"
            },
            Overall = new OverallAssessment
            {
                RecommendSending = true,
                Summary = "Test",
                TopPriorities = new List<string>(),
                NextSteps = "Test"
            },
            ActionableRecommendations = new ActionableRecommendations
            {
                MessageStatus = "Optimal",
                ExecutiveSummary = "Test",
                CanImprove = new List<string>(),
                MustImprove = new List<string>(),
                StrengthsToKeep = new List<string>(),
                FinalRecommendation = "Send"
            }
        };
    }
}
