using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AI
{
    public class ConsequenceAnalyzerService : IConsequenceAnalyzerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<ConsequenceAnalyzerService> _logger;

        public ConsequenceAnalyzerService(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<ConsequenceAnalyzerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            _apiKey = configuration["OpenRouter:ApiKey"] 
                      ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
                      ?? string.Empty;

            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException(
                    "OpenRouter API key not found. Configure it in appsettings.json under 'OpenRouter:ApiKey'");

            _model = configuration["OpenRouter:AnalyzerModel"] ?? "deepseek/deepseek-chat";
        }

        public async Task<ConsequenceAnalysis> AnalyzeAsync(string text, string context = "workplace", string country = "CO")
        {
            ValidateInput(text);

            try
            {
                var prompt = BuildAnalysisPrompt(text, context, country);
                var response = await CallOpenRouterAsync(prompt);
                
                return ParseAnalysisResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al llamar OpenRouter para análisis");
                return GetFallbackAnalysis(text);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al parsear respuesta JSON de IA");
                return GetFallbackAnalysis(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en análisis de consecuencias");
                return GetFallbackAnalysis(text);
            }
        }

        private void ValidateInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("El texto no puede estar vacío", nameof(text));

            if (text.Length < 10)
                throw new ArgumentException("El texto es demasiado corto para analizar (mínimo 10 caracteres)", nameof(text));

            if (text.Length > 5000)
                throw new ArgumentException("El texto excede el límite máximo (5000 caracteres)", nameof(text));
        }

        private string BuildAnalysisPrompt(string text, string context, string country)
        {
            return $@"Analiza este mensaje considerando el contexto colombiano/latinoamericano:

MENSAJE: {text}

CONTEXTO: {context} en {country}

Responde SOLO con un objeto JSON válido (sin markdown, sin texto adicional):

{{
  ""legalRisk"": {{
    ""level"": ""Low|Medium|High|Critical"",
    ""description"": ""Explicación breve"",
    ""potentialIssues"": [""Problema 1""],
    ""legalReferences"": [""Ley aplicable""],
    ""practicalReality"": ""Realidad práctica""
  }},
  ""emotionalImpact"": {{
    ""level"": ""Low|Medium|High|Critical"",
    ""description"": ""Análisis del tono"",
    ""detectedTone"": ""Neutral|Confrontational|Constructive"",
    ""triggerWords"": [""palabra1""],
    ""culturalContext"": ""Contexto cultural""
  }},
  ""effectiveness"": {{
    ""probabilityOfAction"": 50,
    ""reasoning"": ""Por qué"",
    ""missingElements"": [""Qué falta""],
    ""strengthPoints"": [""Qué está bien""],
    ""localRecommendations"": ""Recomendaciones""
  }},
  ""backlash"": {{
    ""level"": ""Low|Medium|High|Critical"",
    ""potentialConsequences"": [""Consecuencia 1""],
    ""mitigationAdvice"": ""Cómo reducir riesgos"",
    ""localProtections"": ""Recursos disponibles""
  }},
  ""overall"": {{
    ""recommendSending"": true,
    ""summary"": ""Evaluación general"",
    ""topPriorities"": [""Prioridad 1"", ""Prioridad 2"", ""Prioridad 3""],
    ""nextSteps"": ""Pasos recomendados""
  }},
  ""actionableRecommendations"": {{
    ""messageStatus"": ""Optimal|NeedsImprovement|Critical"",
    ""executiveSummary"": ""Resumen ejecutivo en 2-3 líneas sobre el estado general del mensaje"",
    ""canImprove"": [""Aspecto opcional que podría mejorar""],
    ""mustImprove"": [""Aspecto crítico que DEBE cambiar""],
    ""strengthsToKeep"": [""Qué está bien y debe mantenerse""],
    ""finalRecommendation"": ""Send|SendWithCaution|Revise|DoNotSend""
  }}
}}";
        }

        private async Task<string> CallOpenRouterAsync(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                max_tokens = 2500
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Add("HTTP-Referer", "https://emailsp.com");
            request.Headers.Add("X-Title", "EmailsP - Consequence Analyzer");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenRouter API returned {response.StatusCode}. Response: {errorContent}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var jsonResponse = await JsonDocument.ParseAsync(stream);

            var responseText = jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return responseText ?? throw new Exception("Respuesta vacía de OpenRouter");
        }

        private ConsequenceAnalysis ParseAnalysisResponse(string jsonResponse)
        {
            try
            {
                _logger.LogInformation("Raw AI response length: {Length}", jsonResponse.Length);

                jsonResponse = jsonResponse.Trim();
                
                if (jsonResponse.StartsWith("```json"))
                    jsonResponse = jsonResponse.Substring(7);
                else if (jsonResponse.StartsWith("```"))
                    jsonResponse = jsonResponse.Substring(3);
                    
                if (jsonResponse.EndsWith("```"))
                    jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3);
                
                jsonResponse = jsonResponse.Trim();

                var jsonStart = jsonResponse.IndexOf('{');
                var jsonEnd = jsonResponse.LastIndexOf('}');
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    jsonResponse = jsonResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }

                var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                return new ConsequenceAnalysis
                {
                    LegalRisk = new LegalRiskAnalysis
                    {
                        Level = Enum.Parse<RiskLevel>(root.GetProperty("legalRisk").GetProperty("level").GetString() ?? "Medium"),
                        Description = root.GetProperty("legalRisk").GetProperty("description").GetString() ?? "",
                        PotentialIssues = ParseStringArray(root.GetProperty("legalRisk").GetProperty("potentialIssues")),
                        LegalReferences = ParseStringArray(root.GetProperty("legalRisk").GetProperty("legalReferences")),
                        PracticalReality = root.GetProperty("legalRisk").TryGetProperty("practicalReality", out var pr) ? pr.GetString() ?? "" : ""
                    },
                    EmotionalImpact = new EmotionalImpactAnalysis
                    {
                        Level = Enum.Parse<RiskLevel>(root.GetProperty("emotionalImpact").GetProperty("level").GetString() ?? "Medium"),
                        Description = root.GetProperty("emotionalImpact").GetProperty("description").GetString() ?? "",
                        DetectedTone = root.GetProperty("emotionalImpact").GetProperty("detectedTone").GetString() ?? "Neutral",
                        TriggerWords = ParseStringArray(root.GetProperty("emotionalImpact").GetProperty("triggerWords")),
                        CulturalContext = root.GetProperty("emotionalImpact").TryGetProperty("culturalContext", out var cc) ? cc.GetString() ?? "" : ""
                    },
                    Effectiveness = new EffectivenessAnalysis
                    {
                        ProbabilityOfAction = root.GetProperty("effectiveness").GetProperty("probabilityOfAction").GetInt32(),
                        Reasoning = root.GetProperty("effectiveness").GetProperty("reasoning").GetString() ?? "",
                        MissingElements = ParseStringArray(root.GetProperty("effectiveness").GetProperty("missingElements")),
                        StrengthPoints = ParseStringArray(root.GetProperty("effectiveness").GetProperty("strengthPoints")),
                        LocalRecommendations = root.GetProperty("effectiveness").TryGetProperty("localRecommendations", out var lr) ? lr.GetString() ?? "" : ""
                    },
                    Backlash = new BacklashAnalysis
                    {
                        Level = Enum.Parse<RiskLevel>(root.GetProperty("backlash").GetProperty("level").GetString() ?? "Medium"),
                        PotentialConsequences = ParseStringArray(root.GetProperty("backlash").GetProperty("potentialConsequences")),
                        MitigationAdvice = root.GetProperty("backlash").GetProperty("mitigationAdvice").GetString() ?? "",
                        LocalProtections = root.GetProperty("backlash").TryGetProperty("localProtections", out var lp) ? lp.GetString() ?? "" : ""
                    },
                    Overall = new OverallAssessment
                    {
                        RecommendSending = root.GetProperty("overall").GetProperty("recommendSending").GetBoolean(),
                        Summary = root.GetProperty("overall").GetProperty("summary").GetString() ?? "",
                        TopPriorities = ParseStringArray(root.GetProperty("overall").GetProperty("topPriorities")),
                        NextSteps = root.GetProperty("overall").TryGetProperty("nextSteps", out var ns) ? ns.GetString() ?? "" : ""
                    },
                    ActionableRecommendations = root.TryGetProperty("actionableRecommendations", out var ar) 
                        ? new ActionableRecommendations
                        {
                            MessageStatus = ar.GetProperty("messageStatus").GetString() ?? "NeedsImprovement",
                            ExecutiveSummary = ar.GetProperty("executiveSummary").GetString() ?? "",
                            CanImprove = ar.TryGetProperty("canImprove", out var ci) ? ParseStringArray(ci) : new List<string>(),
                            MustImprove = ar.TryGetProperty("mustImprove", out var mi) ? ParseStringArray(mi) : new List<string>(),
                            StrengthsToKeep = ar.TryGetProperty("strengthsToKeep", out var stk) ? ParseStringArray(stk) : new List<string>(),
                            FinalRecommendation = ar.GetProperty("finalRecommendation").GetString() ?? "Revise"
                        }
                        : new ActionableRecommendations
                        {
                            MessageStatus = "NeedsImprovement",
                            ExecutiveSummary = "Análisis de recomendaciones no disponible",
                            CanImprove = new List<string>(),
                            MustImprove = new List<string>(),
                            StrengthsToKeep = new List<string>(),
                            FinalRecommendation = "Revise"
                        }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parseando respuesta JSON");
                throw new JsonException("No se pudo parsear la respuesta de la IA", ex);
            }
        }

        private List<string> ParseStringArray(JsonElement element)
        {
            var list = new List<string>();
            foreach (var item in element.EnumerateArray())
            {
                var value = item.GetString();
                if (!string.IsNullOrEmpty(value))
                    list.Add(value);
            }
            return list;
        }

        private ConsequenceAnalysis GetFallbackAnalysis(string text)
        {
            _logger.LogWarning("Usando análisis fallback (rule-based)");

            var hasInsults = text.ToLower().Contains("imbécil") || text.ToLower().Contains("idiota");

            return new ConsequenceAnalysis
            {
                LegalRisk = new LegalRiskAnalysis
                {
                    Level = RiskLevel.Medium,
                    Description = "Análisis básico (IA temporalmente no disponible)",
                    PotentialIssues = new List<string> { "Análisis detallado no disponible" },
                    LegalReferences = new List<string> { "Consultar con abogado laboral" },
                    PracticalReality = "Intente nuevamente en unos minutos"
                },
                EmotionalImpact = new EmotionalImpactAnalysis
                {
                    Level = hasInsults ? RiskLevel.High : RiskLevel.Medium,
                    Description = "Análisis básico de tono",
                    DetectedTone = hasInsults ? "Confrontational" : "Neutral",
                    TriggerWords = new List<string>(),
                    CulturalContext = "No disponible"
                },
                Effectiveness = new EffectivenessAnalysis
                {
                    ProbabilityOfAction = 30,
                    Reasoning = "Estimación básica",
                    MissingElements = new List<string> { "Análisis no disponible" },
                    StrengthPoints = new List<string>(),
                    LocalRecommendations = "Consulte documentación legal colombiana"
                },
                Backlash = new BacklashAnalysis
                {
                    Level = RiskLevel.Medium,
                    PotentialConsequences = new List<string> { "Análisis no disponible" },
                    MitigationAdvice = "Consulte con experto legal",
                    LocalProtections = "Ministerio del Trabajo: 018000 112518"
                },
                Overall = new OverallAssessment
                {
                    RecommendSending = false,
                    Summary = "?? Análisis completo temporalmente no disponible. Reintentar en unos minutos.",
                    TopPriorities = new List<string> { "Reintentar análisis", "Consultar experto legal", "Documentar evidencia" },
                    NextSteps = "Espere unos minutos y vuelva a solicitar el análisis."
                },
                ActionableRecommendations = new ActionableRecommendations
                {
                    MessageStatus = "Critical",
                    ExecutiveSummary = "No se pudo completar el análisis con IA. Se recomienda reintentar en unos minutos para obtener recomendaciones precisas.",
                    CanImprove = new List<string>(),
                    MustImprove = new List<string> { "Obtener análisis completo de IA antes de enviar" },
                    StrengthsToKeep = new List<string>(),
                    FinalRecommendation = "DoNotSend"
                }
            };
        }
    }
}
