using System;
using System.IO;
using System.Linq;
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
                throw new InvalidOperationException("SMTP: falta Smtp:User en configuraci�n.");

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

            var finalHtml = WrapInTemplate(subject ?? string.Empty, bodyHtml ?? string.Empty);
            
            var builder = new BodyBuilder { HtmlBody = finalHtml };

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

                        byte[] fileData;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream, ct);
                            fileData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName) ?? "archivo";
                        builder.Attachments.Add(fileName, fileData);
                        
                        _logger.LogInformation("? Adjunto agregado: {FileName}", fileName);
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
                
                _logger.LogInformation("? Email enviado a {Count} destinatarios con {AttachCount} adjuntos", 
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

        private string WrapInTemplate(string subject, string userContent)
        {
            return $@"<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Correo An�nimo</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Product Sans', 'Roboto', Arial, sans-serif;
            background-color: #1a1a1a;
            padding: 20px;
            min-height: 100vh;
        }}

        .email-container {{
            max-width: 600px;
            width: 100%;
            margin: 0 auto;
            background-color: #242424;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.8);
        }}

        .header {{
            background-color: #2d2d2d;
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
            color: #b0b0b0;
            font-size: 15px;
            line-height: 1.5;
        }}

        .content {{
            padding: 30px;
            background-color: #242424;
        }}

        .message-box {{
            background-color: #2d2d2d;
            border-radius: 8px;
            padding: 24px;
            margin-bottom: 20px;
        }}

        .message-box p {{
            color: #ffffff;
            font-size: 16px;
            line-height: 1.7;
            margin-bottom: 14px;
        }}

        .message-box p:last-child {{
            margin-bottom: 0;
        }}

        .message-box h1,
        .message-box h2,
        .message-box h3 {{
            color: #ffffff;
            margin-bottom: 12px;
        }}

        .message-box strong {{
            color: #8AB4F8;
            font-weight: 600;
        }}

        .message-box em {{
            color: #8AB4F8;
            font-style: italic;
        }}

        .message-box ul,
        .message-box ol {{
            color: #ffffff;
            margin-left: 20px;
            margin-bottom: 12px;
        }}

        .message-box li {{
            margin-bottom: 6px;
            line-height: 1.6;
        }}

        .message-box a {{
            color: #8AB4F8;
            text-decoration: none;
        }}

        .message-box a:hover {{
            text-decoration: underline;
        }}

        .footer {{
            background-color: #2d2d2d;
            padding: 20px 30px;
            border-top: 1px solid #3d3d3d;
            text-align: center;
        }}

        .footer p {{
            color: #a0a0a0;
            font-size: 12px;
            line-height: 1.5;
        }}

        .badge {{
            display: inline-block;
            background-color: #3d3d3d;
            color: #b0b0b0;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-top: 8px;
        }}

        @media screen and (max-width: 640px) {{
            body {{
                padding: 10px;
            }}

            .header {{
                padding: 30px 20px;
            }}

            .incognito-icon {{
                width: 72px;
                height: 72px;
                font-size: 48px;
            }}

            .header h1 {{
                font-size: 24px;
            }}

            .header p {{
                font-size: 14px;
            }}

            .content {{
                padding: 20px !important;
            }}

            .message-box {{
                padding: 18px !important;
            }}

            .message-box p {{
                font-size: 15px !important;
            }}

            .footer {{
                padding: 20px !important;
            }}

            .footer p {{
                font-size: 12px;
            }}
        }}

        @media screen and (max-width: 480px) {{
            body {{
                padding: 5px;
            }}

            .email-container {{
                border-radius: 8px;
            }}

            .header {{
                padding: 24px 16px;
            }}

            .incognito-icon {{
                width: 64px;
                height: 64px;
                font-size: 40px;
            }}

            .header h1 {{
                font-size: 22px;
            }}

            .header p {{
                font-size: 13px;
            }}

            .content {{
                padding: 16px !important;
            }}

            .message-box {{
                padding: 16px !important;
                border-radius: 6px;
            }}

            .message-box p {{
                font-size: 14px !important;
            }}

            .badge {{
                font-size: 11px;
                padding: 5px 12px;
            }}

            .footer {{
                padding: 16px !important;
            }}
        }}

        @media screen and (max-width: 360px) {{
            .header h1 {{
                font-size: 20px;
            }}

            .incognito-icon {{
                width: 56px;
                height: 56px;
                font-size: 36px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""icon-container"">
                <div class=""incognito-icon"">???</div>
            </div>
            <h1>Mensaje An�nimo</h1>
            <p>Este correo ha sido enviado de forma privada</p>
        </div>

        <div class=""content"">
            <div class=""message-box"">
                {userContent}
            </div>

            <span class=""badge"">Modo Privado Activo</span>
        </div>

        <div class=""footer"">
            <p>Este mensaje fue enviado a trav�s de un servicio de correo an�nimo.</p>
            <p>No respondas directamente a este correo. Si deseas responder, utiliza el servicio correspondiente.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
