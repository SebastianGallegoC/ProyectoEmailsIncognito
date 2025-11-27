using Application.DTOs;
using Application.Services;
using Domain.Interfaces;
using EmailsP.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailsP.Controllers
{
    [ApiController]
    [Route("[controllerjsjs]")]
    [Produces("application/json")]
    public class EmailController : ControllerBase
    {
        private readonly EmailSenderUseCase _useCase;
        private readonly IContactRepository _contacts;
        private readonly ILogger<EmailController> _logger;
        private readonly IWebHostEnvironment _env;

        public EmailController(
            EmailSenderUseCase useCase,
            IContactRepository contacts,
            ILogger<EmailController> logger,
            IWebHostEnvironment env)
        {
            _useCase = useCase;
            _contacts = contacts;
            _logger = logger;
            _env = env;
        }

        [HttpPost("Send")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public async Task<IActionResult> SendEmail([FromForm] EmailRequest request)
        {
            _logger.LogInformation("=== INICIO SendEmail ===");
            _logger.LogInformation("Destinatarios To: {Count}", request.To?.Count ?? 0);
            _logger.LogInformation("Adjuntos: {Count}", request.Attachments?.Count ?? 0);

            // 1) Resolver destinatarios (To + nombres + ids)
            var recipients = new List<string>();

            if (request.To != null && request.To.Count > 0)
                recipients.AddRange(request.To.Where(x => !string.IsNullOrWhiteSpace(x)));

            if (request.ToContactNames != null && request.ToContactNames.Count > 0)
            {
                var usuarioId = User.GetUsuarioId();
                var emailsByNames = await _contacts.GetEmailsByNamesAsync(usuarioId, request.ToContactNames, allowPartialMatch: true);
                recipients.AddRange(emailsByNames);
                if (emailsByNames.Count == 0)
                    return BadRequest($"No se hallaron correos para los nombres: {string.Join(", ", request.ToContactNames)}");
            }

            if (request.ToContactIds != null && request.ToContactIds.Count > 0)
            {
                var usuarioId = User.GetUsuarioId();
                var emailsByIds = await _contacts.GetEmailsByIdsAsync(usuarioId, request.ToContactIds);
                recipients.AddRange(emailsByIds);
                if (emailsByIds.Count == 0)
                    return BadRequest("Ningún ID de contacto resolvió un correo.");
            }

            recipients = recipients
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (recipients.Count == 0)
                return BadRequest("Debe proporcionar al menos un destinatario (To, ToContactNames o ToContactIds).");

            _logger.LogInformation("Destinatarios finales: {Recipients}", string.Join(", ", recipients));

            // 2) Enviar CON adjuntos
            try
            {
                _logger.LogInformation("Llamando a UseCase.ExecuteAsync...");
                await _useCase.ExecuteAsync(recipients, request.Subject, request.Body, request.Attachments);
                _logger.LogInformation("=== FIN SendEmail EXITOSO ===");
                return Ok($"✅ Email enviado a {recipients.Count} destinatario(s) con {request.Attachments?.Count ?? 0} adjunto(s)");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error controlado en envío.");
                var detail = _env.IsDevelopment() ? Flatten(ex) : ex.Message;
                return Problem(statusCode: StatusCodes.Status502BadGateway,
                               title: "Error al enviar",
                               detail: detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar.");
                var detail = _env.IsDevelopment() ? Flatten(ex) : "Ocurrió un error al enviar el correo.";
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                               title: "Error inesperado",
                               detail: detail);
            }
        }

        /// <summary>
        /// Endpoint de prueba para envío sin adjuntos
        /// </summary>
        [HttpPost("SendSimple")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendSimpleEmail([FromBody] SimpleEmailRequest request)
        {
            _logger.LogInformation("=== SendSimple Email ===");
            
            if (string.IsNullOrWhiteSpace(request.To))
                return BadRequest("El campo 'To' es requerido");

            try
            {
                await _useCase.ExecuteAsync(
                    new[] { request.To }, 
                    request.Subject ?? "Test", 
                    request.Body ?? "<p>Test</p>", 
                    null);
                
                return Ok("✅ Email simple enviado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email simple");
                return Problem(detail: ex.Message);
            }
        }

        private static string Flatten(Exception ex)
        {
            var parts = new List<string>();
            for (var e = ex; e != null; e = e.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(e.Message)) parts.Add(e.Message.Trim());
            }
            return string.Join(" | ", parts.Distinct());
        }
    }

    public class SimpleEmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
