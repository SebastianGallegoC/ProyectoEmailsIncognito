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
        private readonly TextRefactorUseCase _useCase;
        private readonly ILogger<AIController> _logger;
        private readonly IWebHostEnvironment _env;

        public AIController(
            TextRefactorUseCase useCase, 
            ILogger<AIController> logger,
            IWebHostEnvironment env)
        {
            _useCase = useCase;
            _logger = logger;
            _env = env;
        }

        [HttpPost("refactor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Refactor([FromBody] RefactorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text cannot be empty." });

            try
            {
                var result = await _useCase.ExecuteAsync(request.Text);

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
    }
}
