using Application.DTOs.AI;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailsP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AIController : ControllerBase
    {
        private readonly TextRefactorUseCase _refactorUseCase;
        private readonly ConsequenceAnalyzerUseCase _analyzerUseCase;
        private readonly ILogger<AIController> _logger;
        private readonly IWebHostEnvironment _env;

        public AIController(
            TextRefactorUseCase refactorUseCase,
            ConsequenceAnalyzerUseCase analyzerUseCase,
            ILogger<AIController> logger,
            IWebHostEnvironment env)
        {
            _refactorUseCase = refactorUseCase;
            _analyzerUseCase = analyzerUseCase;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Reformula un texto para hacerlo más formal y profesional
        /// </summary>
        [HttpPost("refactor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Refactor([FromBody] RefactorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text cannot be empty." });

            try
            {
                var result = await _refactorUseCase.ExecuteAsync(request.Text);

                return Ok(new RefactorResponse
                {
                    FormalText = result
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Configuration error during text refactoring.");
                var detail = _env.IsDevelopment() ? ex.Message : "Configuration error.";
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Configuration Error",
                    detail: detail
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling OpenRouter API.");
                var detail = _env.IsDevelopment()
                    ? $"OpenRouter API error: {ex.Message}"
                    : "Error communicating with AI service.";
                return Problem(
                    statusCode: StatusCodes.Status502BadGateway,
                    title: "AI Service Error",
                    detail: detail
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during text refactoring.");
                var detail = _env.IsDevelopment() ? ex.Message : "An error occurred while refactoring the text.";
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unexpected Error",
                    detail: detail
                );
            }
        }

        /// <summary>
        /// Analiza las consecuencias de enviar un mensaje anónimo
        /// Especializado en contexto legal y cultural de Colombia y Latinoamérica
        /// </summary>
        [HttpPost("analyze-consequences")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AnalyzeConsequences([FromBody] AnalyzeConsequencesRequest request)
        {
            // Log para debugging
            _logger.LogInformation("Analyze request received. Request null: {IsNull}", request == null);
            
            if (request == null)
            {
                _logger.LogWarning("Request is null - binding failed");
                return BadRequest(new { error = "Request body could not be parsed. Check JSON format." });
            }

            _logger.LogInformation("Request received - Text length: {Length}, Context: {Context}, Country: {Country}", 
                request.Text?.Length ?? 0, request.Context, request.Country);

            // Validación manual
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { error = "Text is required and cannot be empty" });
            }

            if (request.Text.Length < 10)
            {
                return BadRequest(new { error = "Text must be at least 10 characters long" });
            }

            if (request.Text.Length > 5000)
            {
                return BadRequest(new { error = "Text cannot exceed 5000 characters" });
            }

            try
            {
                var result = await _analyzerUseCase.ExecuteAsync(
                    request.Text,
                    request.Context ?? "workplace",
                    request.Country ?? "CO");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for consequence analysis");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Configuration error during consequence analysis.");
                var detail = _env.IsDevelopment() ? ex.Message : "Configuration error.";
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Configuration Error",
                    detail: detail
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling AI service for analysis.");
                var detail = _env.IsDevelopment()
                    ? $"AI Service error: {ex.Message}"
                    : "Error communicating with AI service.";
                return Problem(
                    statusCode: StatusCodes.Status502BadGateway,
                    title: "AI Service Error",
                    detail: detail
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during consequence analysis.");
                var detail = _env.IsDevelopment() ? ex.Message : "An error occurred while analyzing consequences.";
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unexpected Error",
                    detail: detail
                );
            }
        }
    }
}
