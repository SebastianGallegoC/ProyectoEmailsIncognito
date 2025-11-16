using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AI
{
    public class OpenRouterAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenRouterAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            
            // Intenta leer desde configuración primero, luego desde variable de entorno
            _apiKey = configuration["OpenRouter:ApiKey"] 
                      ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
                      ?? string.Empty;

            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException(
                    "OpenRouter API key not found. Configure it in appsettings.json under 'OpenRouter:ApiKey' " +
                    "or set the OPENROUTER_API_KEY environment variable.");

            // Lee el modelo desde configuración, por defecto usa Google Gemini 2.0 Flash (gratis)
            _model = configuration["OpenRouter:Model"] ?? "google/gemini-2.0-flash-exp:free";
        }

        public async Task<string> RefactorTextAsync(string text)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "Eres un asistente que reescribe textos de forma más clara, profesional y formal sin alterar su significado. Responde SOLO con el texto reformulado, sin explicaciones adicionales." },
                    new { role = "user", content = $"Reformula este texto de manera profesional y formal:\n\n{text}" }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Add("HTTP-Referer", "https://emailsp.com");
            request.Headers.Add("X-Title", "EmailsP - AI Refactor Service");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"OpenRouter API returned {response.StatusCode}. " +
                    $"Response: {errorContent}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var jsonResponse = await JsonDocument.ParseAsync(stream);

            var formalText = jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return formalText ?? text; // Fallback al texto original si hay problema
        }
    }
}
