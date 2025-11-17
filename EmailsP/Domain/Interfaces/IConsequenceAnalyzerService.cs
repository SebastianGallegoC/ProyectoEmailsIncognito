using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Servicio de IA para analizar consecuencias de mensajes anónimos
    /// Especializado en contexto legal y cultural de Colombia y Latinoamérica
    /// </summary>
    public interface IConsequenceAnalyzerService
    {
        /// <summary>
        /// Analiza un texto y predice sus consecuencias legales, emocionales y de efectividad
        /// </summary>
        /// <param name="text">Texto a analizar</param>
        /// <param name="context">Contexto del mensaje (workplace, academic, personal)</param>
        /// <param name="country">Código del país (CO, PE, EC, etc.)</param>
        /// <returns>Análisis completo de consecuencias</returns>
        Task<ConsequenceAnalysis> AnalyzeAsync(string text, string context = "workplace", string country = "CO");
    }

    /// <summary>
    /// Resultado del análisis de consecuencias
    /// </summary>
    public class ConsequenceAnalysis
    {
        public LegalRiskAnalysis LegalRisk { get; set; } = new();
        public EmotionalImpactAnalysis EmotionalImpact { get; set; } = new();
        public EffectivenessAnalysis Effectiveness { get; set; } = new();
        public BacklashAnalysis Backlash { get; set; } = new();
        public OverallAssessment Overall { get; set; } = new();
        public ActionableRecommendations ActionableRecommendations { get; set; } = new();
    }

    /// <summary>
    /// Análisis de riesgo legal
    /// </summary>
    public class LegalRiskAnalysis
    {
        public RiskLevel Level { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> PotentialIssues { get; set; } = new();
        public List<string> LegalReferences { get; set; } = new();
        public string PracticalReality { get; set; } = string.Empty;
    }

    /// <summary>
    /// Análisis de impacto emocional
    /// </summary>
    public class EmotionalImpactAnalysis
    {
        public RiskLevel Level { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DetectedTone { get; set; } = string.Empty;
        public List<string> TriggerWords { get; set; } = new();
        public string CulturalContext { get; set; } = string.Empty;
    }

    /// <summary>
    /// Análisis de efectividad del mensaje
    /// </summary>
    public class EffectivenessAnalysis
    {
        public int ProbabilityOfAction { get; set; } // 0-100
        public string Reasoning { get; set; } = string.Empty;
        public List<string> MissingElements { get; set; } = new();
        public List<string> StrengthPoints { get; set; } = new();
        public string LocalRecommendations { get; set; } = string.Empty;
    }

    /// <summary>
    /// Análisis de riesgo de backlash/represalias
    /// </summary>
    public class BacklashAnalysis
    {
        public RiskLevel Level { get; set; }
        public List<string> PotentialConsequences { get; set; } = new();
        public string MitigationAdvice { get; set; } = string.Empty;
        public string LocalProtections { get; set; } = string.Empty;
    }

    /// <summary>
    /// Evaluación general y recomendaciones
    /// </summary>
    public class OverallAssessment
    {
        public bool RecommendSending { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<string> TopPriorities { get; set; } = new();
        public string NextSteps { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resumen estructurado de recomendaciones accionables
    /// </summary>
    public class ActionableRecommendations
    {
        public string MessageStatus { get; set; } = string.Empty;
        public string ExecutiveSummary { get; set; } = string.Empty;
        public List<string> CanImprove { get; set; } = new();
        public List<string> MustImprove { get; set; } = new();
        public List<string> StrengthsToKeep { get; set; } = new();
        public string FinalRecommendation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Niveles de riesgo
    /// </summary>
    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}
