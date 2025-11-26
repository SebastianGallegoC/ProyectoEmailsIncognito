using Application.Services;
using Domain.Interfaces;
using Infrastructure.AI;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    try
    {
        var ex = e.ExceptionObject as Exception;
        Console.WriteLine($"[FATAL][AppDomain] {ex}");
    }
    catch { }
};
TaskScheduler.UnobservedTaskException += (s, e) =>
{
    try
    {
        Console.WriteLine($"[ERROR][UnobservedTask] {e.Exception}");
        e.SetObserved();
    }
    catch { }
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Emails API", Version = "v1" });

    var jwt = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Pega SOLO el token (Swagger agregar� 'Bearer ').",
        Reference = new OpenApiReference { Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme }
    };

    options.AddSecurityDefinition(jwt.Reference.Id, jwt);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwt, Array.Empty<string>() } });
});

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100 * 1024 * 1024;
    o.ValueLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 100L * 1024L * 1024L;
});
builder.Services.Configure<IISServerOptions>(o =>
{
    o.MaxRequestBodySize = 100L * 1024L * 1024L;
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("DevAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "clave-secreta-larga-por-defecto");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IEmailService, GmailSenderService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<EmailSenderUseCase>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();

builder.Services.AddScoped<IContactRepository, ContactRepositoryPostgres>();
builder.Services.AddScoped<ContactService>();

builder.Services.AddHttpClient<IAIService, OpenRouterAIService>();
builder.Services.AddScoped<TextRefactorUseCase>();

builder.Services.AddHttpClient<IConsequenceAnalyzerService, ConsequenceAnalyzerService>();
builder.Services.AddScoped<ConsequenceAnalyzerUseCase>();

var app = builder.Build();

// Habilitar CORS siempre (Development y Production)
app.UseCors("DevAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var payload = new { title = "Error inesperado", detail = ex?.Message, status = 500 };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", ctx => { ctx.Response.Redirect("/compose.html"); return Task.CompletedTask; });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
