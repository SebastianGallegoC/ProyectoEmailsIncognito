namespace Application.DTOs.AI
{
    /// <summary>
    /// Response del análisis de consecuencias
    /// </summary>
    public class ConsequenceAnalysisResponse
    {
        public LegalRiskDto LegalRisk { get; set; } = new();
        public EmotionalImpactDto EmotionalImpact { get; set; } = new();
        public EffectivenessDto Effectiveness { get; set; } = new();
        public BacklashDto Backlash { get; set; } = new();
        public OverallAssessmentDto Overall { get; set; } = new();
        public ActionableRecommendationsDto ActionableRecommendations { get; set; } = new();
    }

    public class LegalRiskDto
    {
        public string Level { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> PotentialIssues { get; set; } = new();
        public List<string> LegalReferences { get; set; } = new();
        public string PracticalReality { get; set; } = string.Empty;
    }

    public class EmotionalImpactDto
    {
        public string Level { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DetectedTone { get; set; } = string.Empty;
        public List<string> TriggerWords { get; set; } = new();
        public string CulturalContext { get; set; } = string.Empty;
    }

    public class EffectivenessDto
    {
        public int ProbabilityOfAction { get; set; }
        public string Reasoning { get; set; } = string.Empty;
        public List<string> MissingElements { get; set; } = new();
        public List<string> StrengthPoints { get; set; } = new();
        public string LocalRecommendations { get; set; } = string.Empty;
    }

    public class BacklashDto
    {
        public string Level { get; set; } = string.Empty;
        public List<string> PotentialConsequences { get; set; } = new();
        public string MitigationAdvice { get; set; } = string.Empty;
        public string LocalProtections { get; set; } = string.Empty;
    }

    public class OverallAssessmentDto
    {
        public bool RecommendSending { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<string> TopPriorities { get; set; } = new();
        public string NextSteps { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resumen estructurado de recomendaciones accionables
    /// </summary>
    public class ActionableRecommendationsDto
    {
        /// <summary>
        /// Estado general del mensaje: Optimal, NeedsImprovement, Critical
        /// </summary>
        public string MessageStatus { get; set; } = string.Empty;

        /// <summary>
        /// Resumen ejecutivo (2-3 líneas)
        /// </summary>
        public string ExecutiveSummary { get; set; } = string.Empty;

        /// <summary>
        /// Qué se puede mejorar (opcional)
        /// </summary>
        public List<string> CanImprove { get; set; } = new();

        /// <summary>
        /// Qué se DEBE mejorar (crítico)
        /// </summary>
        public List<string> MustImprove { get; set; } = new();

        /// <summary>
        /// Qué está bien en el mensaje
        /// </summary>
        public List<string> StrengthsToKeep { get; set; } = new();

        /// <summary>
        /// Recomendación final: Send, SendWithCaution, Revise, DoNotSend
        /// </summary>
        public string FinalRecommendation { get; set; } = string.Empty;
    }
}
