using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Domain.Interfaces;

using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Infrastructure.Services
{
    public class GmailSenderService : IEmailService
    {
        private readonly ILogger<GmailSenderService> _logger;
        private readonly IConfiguration _cfg;

        public GmailSenderService(ILogger<GmailSenderService> logger, IConfiguration cfg)
        {
            _logger = logger;
            _cfg = cfg;
        }

        public async Task SendAsync(
            IEnumerable<string> to,
            string subject,
            string? bodyHtml,
            IEnumerable<IFormFile>? attachments,
            CancellationToken ct = default)
        {
            var user = _cfg["Smtp:User"];
            if (string.IsNullOrWhiteSpace(user))
                throw new InvalidOperationException("SMTP: falta Smtp:User en configuración.");

            var host = _cfg["Smtp:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
            var useStartTls = bool.TryParse(_cfg["Smtp:UseStartTls"], out var st) ? st : true;
            var secure = useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;

            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(user));

            foreach (var addr in (to ?? Enumerable.Empty<string>())
                                 .Where(a => !string.IsNullOrWhiteSpace(a))
                                 .Distinct(StringComparer.OrdinalIgnoreCase))
                msg.To.Add(MailboxAddress.Parse(addr));

            msg.Subject = subject ?? string.Empty;

            // Usar el template HTML con el contenido del usuario
            var finalHtml = WrapInTemplate(bodyHtml ?? string.Empty);
            
            var builder = new BodyBuilder { HtmlBody = finalHtml };

            // Procesar adjuntos de forma segura
            if (attachments != null)
            {
                var files = attachments.ToList();
                _logger.LogInformation("Procesando {Count} adjuntos", files.Count);

                foreach (var file in files)
                {
                    if (file == null || file.Length == 0) continue;

                    try
                    {
                        _logger.LogInformation("Agregando adjunto: {FileName} ({Size} bytes)", 
                            file.FileName, file.Length);

                        // Estrategia: Leer TODO el archivo de una vez a memoria
                        byte[] fileData;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream, ct);
                            fileData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName) ?? "archivo";
                        builder.Attachments.Add(fileName, fileData);
                        
                        _logger.LogInformation("✅ Adjunto agregado: {FileName}", fileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error procesando adjunto {FileName}", file.FileName);
                        throw new InvalidOperationException($"Error procesando adjunto '{file.FileName}': {ex.Message}", ex);
                    }
                }
            }

            msg.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            
            try
            {
                client.Timeout = 60000;
                
                await client.ConnectAsync(host, port, secure, ct);
                await client.AuthenticateAsync(user, _cfg["Smtp:Password"], ct);
                await client.SendAsync(msg, ct);
                
                _logger.LogInformation("✅ Email enviado a {Count} destinatarios con {AttachCount} adjuntos", 
                    msg.To.Count, attachments?.Count() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email");
                throw new InvalidOperationException($"Error SMTP: {ex.Message}", ex);
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true, CancellationToken.None);
            }
        }

        private string WrapInTemplate(string userContent)
        {
            return $@"<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Correo Anónimo</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Product Sans', 'Roboto', Arial, sans-serif;
            background-color: #292a2d;
            padding: 20px;
        }}

        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #35363a;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.5);
        }}

        .header {{
            background-color: #3c4043;
            padding: 40px 30px;
            text-align: center;
        }}

        .icon-container {{
            margin-bottom: 24px;
        }}

        .incognito-icon {{
            width: 96px;
            height: 96px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 64px;
            filter: grayscale(100%);
            opacity: 0.9;
        }}

        .header h1 {{
            color: #ffffff;
            font-size: 28px;
            font-weight: 400;
            margin-bottom: 12px;
            letter-spacing: -0.5px;
        }}

        .header p {{
            color: #bdc1c6;
            font-size: 15px;
            line-height: 1.5;
        }}

        .content {{
            padding: 30px;
        }}

        .message-box {{
            background-color: #3c4043;
            border-radius: 8px;
            padding: 24px;
            margin-bottom: 20px;
        }}

        .message-box p {{
            color: #e8eaed;
            font-size: 16px;
            line-height: 1.7;
            margin-bottom: 14px;
        }}

        .message-box p:last-child {{
            margin-bottom: 0;
        }}

        .user-content {{
            background-color: #292a2d;
            border-radius: 8px;
            padding: 24px;
            margin-top: 20px;
            border-left: 4px solid #8ab4f8;
        }}

        .user-content h1,
        .user-content h2,
        .user-content h3 {{
            color: #e8eaed;
            margin-bottom: 12px;
        }}

        .user-content p {{
            color: #e8eaed;
            font-size: 15px;
            line-height: 1.7;
            margin-bottom: 12px;
        }}

        .user-content strong {{
            color: #8ab4f8;
        }}

        .user-content ul,
        .user-content ol {{
            color: #e8eaed;
            margin-left: 20px;
            margin-bottom: 12px;
        }}

        .footer {{
            background-color: #35363a;
            padding: 20px 30px;
            border-top: 1px solid #3c4043;
            text-align: center;
        }}

        .footer p {{
            color: #9aa0a6;
            font-size: 12px;
            line-height: 1.5;
        }}

        .badge {{
            display: inline-block;
            background-color: #3c4043;
            color: #9aa0a6;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-top: 8px;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""icon-container"">
                <div class=""incognito-icon"">🕶️</div>
            </div>
            <h1>Mensaje Anónimo</h1>
            <p>Este correo ha sido enviado de forma privada</p>
        </div>

        <div class=""content"">
            <div class=""message-box"">
                <p><strong>Hola,</strong></p>
                <p>Has recibido este mensaje a través de nuestro servicio de correo anónimo. La identidad del remitente está protegida para garantizar su privacidad.</p>
                <p>El contenido de este mensaje es confidencial y está destinado únicamente para ti.</p>
            </div>

            <div class=""user-content"">
                {userContent}
            </div>

            <span class=""badge"">Modo Privado Activo</span>
        </div>

        <div class=""footer"">
            <p>Este mensaje fue enviado a través de un servicio de correo anónimo.</p>
            <p>No respondas directamente a este correo. Si deseas responder, utiliza el servicio correspondiente.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
