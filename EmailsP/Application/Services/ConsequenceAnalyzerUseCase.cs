using Domain.Interfaces;
using Application.DTOs.AI;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <summary>
    /// Caso de uso para analizar consecuencias de mensajes anónimos
    /// </summary>
    public class ConsequenceAnalyzerUseCase
    {
        private readonly IConsequenceAnalyzerService _analyzerService;
        private readonly ILogger<ConsequenceAnalyzerUseCase> _logger;

        public ConsequenceAnalyzerUseCase(
            IConsequenceAnalyzerService analyzerService,
            ILogger<ConsequenceAnalyzerUseCase> logger)
        {
            _analyzerService = analyzerService;
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta el análisis de consecuencias
        /// </summary>
        public async Task<ConsequenceAnalysisResponse> ExecuteAsync(
            string text, 
            string context = "workplace", 
            string country = "CO")
        {
            _logger.LogInformation("Analizando consecuencias para texto de {Length} caracteres, país: {Country}", 
                text.Length, country);

            try
            {
                var analysis = await _analyzerService.AnalyzeAsync(text, context, country);
                
                return MapToDto(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar consecuencias");
                throw;
            }
        }

        private static ConsequenceAnalysisResponse MapToDto(ConsequenceAnalysis analysis)
        {
            return new ConsequenceAnalysisResponse
            {
                LegalRisk = new LegalRiskDto
                {
                    Level = analysis.LegalRisk.Level.ToString(),
                    Description = analysis.LegalRisk.Description,
                    PotentialIssues = analysis.LegalRisk.PotentialIssues,
                    LegalReferences = analysis.LegalRisk.LegalReferences,
                    PracticalReality = analysis.LegalRisk.PracticalReality
                },
                EmotionalImpact = new EmotionalImpactDto
                {
                    Level = analysis.EmotionalImpact.Level.ToString(),
                    Description = analysis.EmotionalImpact.Description,
                    DetectedTone = analysis.EmotionalImpact.DetectedTone,
                    TriggerWords = analysis.EmotionalImpact.TriggerWords,
                    CulturalContext = analysis.EmotionalImpact.CulturalContext
                },
                Effectiveness = new EffectivenessDto
                {
                    ProbabilityOfAction = analysis.Effectiveness.ProbabilityOfAction,
                    Reasoning = analysis.Effectiveness.Reasoning,
                    MissingElements = analysis.Effectiveness.MissingElements,
                    StrengthPoints = analysis.Effectiveness.StrengthPoints,
                    LocalRecommendations = analysis.Effectiveness.LocalRecommendations
                },
                Backlash = new BacklashDto
                {
                    Level = analysis.Backlash.Level.ToString(),
                    PotentialConsequences = analysis.Backlash.PotentialConsequences,
                    MitigationAdvice = analysis.Backlash.MitigationAdvice,
                    LocalProtections = analysis.Backlash.LocalProtections
                },
                Overall = new OverallAssessmentDto
                {
                    RecommendSending = analysis.Overall.RecommendSending,
                    Summary = analysis.Overall.Summary,
                    TopPriorities = analysis.Overall.TopPriorities,
                    NextSteps = analysis.Overall.NextSteps
                },
                ActionableRecommendations = new ActionableRecommendationsDto
                {
                    MessageStatus = analysis.ActionableRecommendations.MessageStatus,
                    ExecutiveSummary = analysis.ActionableRecommendations.ExecutiveSummary,
                    CanImprove = analysis.ActionableRecommendations.CanImprove,
                    MustImprove = analysis.ActionableRecommendations.MustImprove,
                    StrengthsToKeep = analysis.ActionableRecommendations.StrengthsToKeep,
                    FinalRecommendation = analysis.ActionableRecommendations.FinalRecommendation
                }
            };
        }
    }
}
