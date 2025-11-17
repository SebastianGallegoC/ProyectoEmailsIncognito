# 🔧 EmailsP - Backend API (.NET)

API RESTful para envío de correos electrónicos anónimos con arquitectura limpia (Clean Architecture).

## 🏗️ Arquitectura

```
EmailsP/
├── Domain/                      # Capa de Dominio
│   ├── Entities/               # Entidades del negocio
│   │   ├── Usuario.cs
│   │   └── Contact.cs
│   └── Interfaces/             # Contratos
│       ├── IEmailService.cs
│       ├── IUsuarioRepository.cs
│       ├── IContactRepository.cs
│       └── IAIService.cs
│
├── Application/                 # Capa de Aplicación
│   ├── DTOs/                   # Data Transfer Objects
│   │   ├── LoginRequest.cs
│   │   ├── EmailRequest.cs
│   │   └── AI/
│   └── Services/               # Casos de uso
│       ├── AuthService.cs
│       ├── EmailSenderUseCase.cs
│       └── ContactService.cs
│
├── Infraestructure/            # Capa de Infraestructura
│   ├── Services/               # Implementaciones
│   │   ├── UsuarioRepository.cs
│   │   ├── ContactRepositoryPostgres.cs
│   │   └── EmailRepository.cs
│   ├── AI/                     # Servicios de IA
│   │   ├── OpenRouterAIService.cs
│   │   └── ConsequenceAnalyzerService.cs
│   ├── GmailSenderService.cs   # Servicio de Gmail
│   └── Templates/              # Templates HTML
│
└── EmailsP/                    # Capa de Presentación
    ├── Controllers/            # API Controllers
    │   ├── AuthController.cs
    │   ├── EmailController.cs
    │   ├── ContactsController.cs
    │   └── AIController.cs
    ├── Extensions/
    └── Program.cs              # Configuración de servicios
```

## 📋 Requisitos

- **.NET 8 SDK**
- **PostgreSQL 14+**
- **Cuenta de Gmail** con OAuth2
- **API Key de OpenRouter** (opcional - para IA)

## 🚀 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/SebastianGallegoC/ProyectoEmailsIncognito.git
cd ProyectoEmailsIncognito/EmailsP
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

Las dependencias se restauran automáticamente al compilar.

### 3. Configurar Base de Datos

#### Crear base de datos PostgreSQL:

```sql
CREATE DATABASE emailsp;
```

#### Ejecutar scripts de tablas:

```sql
-- Tabla usuarios
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla contactos
CREATE TABLE contacts (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES usuarios(id),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    is_favorite BOOLEAN DEFAULT FALSE,
    is_blocked BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Usuario por defecto (password: admin123)
INSERT INTO usuarios (username, password_hash, email) 
VALUES ('admin', '$2a$11$xKzVHj7Zj9L9YmZQZMqZJOy8K7pY5rL9wZ7xYmZQZMqZJOy8K7pY5', 'admin@emailsp.com');
```

### 4. Configurar variables de entorno

Crear `EmailsP/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=emailsp;Username=postgres;Password=tu_password"
  },
  "JwtSettings": {
    "Secret": "clave-super-secreta-de-al-menos-32-caracteres-para-jwt-tokens",
    "Issuer": "EmailsP",
    "Audience": "EmailsPClient",
    "ExpirationHours": 24
  },
  "OpenRouter": {
    "ApiKey": "sk-or-v1-tu-api-key-aqui",
    "BaseUrl": "https://openrouter.ai/api/v1"
  },
  "Gmail": {
    "FromEmail": "tu-correo@gmail.com",
    "FromName": "EmailsP Anónimo"
  }
}
```

### 5. Configurar Gmail OAuth2

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un nuevo proyecto
3. Habilita **Gmail API**
4. Ve a **Credenciales** → **Crear credenciales** → **ID de cliente de OAuth 2.0**
5. Tipo de aplicación: **Aplicación de escritorio**
6. Descarga el JSON de credenciales
7. Guárdalo en `Infraestructure/credentials/credentials.json`

Estructura del archivo:
```json
{
  "installed": {
    "client_id": "tu-client-id.apps.googleusercontent.com",
    "project_id": "tu-proyecto",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "tu-client-secret",
    "redirect_uris": ["http://localhost"]
  }
}
```

### 6. Ejecutar el proyecto

```bash
dotnet run --project EmailsP/EmailsP.csproj
```

La API estará disponible en:
- **HTTP**: http://localhost:5162
- **Swagger UI**: http://localhost:5162/swagger

## 📚 Endpoints Principales

### 🔐 Autenticación (`/api/Auth`)

#### POST `/api/Auth/login`
Login de usuario
```json
{
  "username": "admin",
  "password": "admin123"
}
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@emailsp.com"
}
```

#### POST `/api/Auth/register`
Registro de nuevo usuario
```json
{
  "username": "nuevo_usuario",
  "password": "password123",
  "email": "usuario@example.com"
}
```

### 📧 Emails (`/Email`)

#### POST `/Email/Send`
Enviar email con adjuntos (requiere autenticación)

**Content-Type**: `multipart/form-data`

```
To: email1@example.com
To: email2@example.com
Subject: Asunto del correo
Body: Contenido del mensaje
Attachments: [archivos]
```

#### POST `/Email/SendSimple`
Enviar email simple sin adjuntos

```json
{
  "to": "destinatario@example.com",
  "subject": "Asunto",
  "body": "<p>Contenido HTML</p>"
}
```

### 👥 Contactos (`/api/Contacts`)

#### GET `/api/Contacts`
Obtener contactos paginados
- Query params: `page`, `pageSize`, `search`

#### POST `/api/Contacts`
Crear nuevo contacto
```json
{
  "name": "Juan Pérez",
  "email": "juan@example.com"
}
```

#### PUT `/api/Contacts/{id}`
Actualizar contacto

#### DELETE `/api/Contacts/{id}`
Eliminar contacto

#### POST `/api/Contacts/{id}/favorite`
Marcar como favorito

#### POST `/api/Contacts/{id}/block`
Bloquear contacto

### 🤖 IA (`/api/AI`)

#### POST `/api/AI/refactor`
Reformular texto a formal
```json
{
  "text": "oye amigo como estas todo bien?"
}
```

#### POST `/api/AI/analyze-consequences`
Analizar consecuencias del mensaje
```json
{
  "message": "Texto a analizar"
}
```

## 🔧 Configuración de Servicios

### Program.cs - Inyección de dependencias

```csharp
// Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

// Servicios de negocio
builder.Services.AddScoped<IEmailService, GmailSenderService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepositoryPostgres>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailSenderUseCase>();

// Servicios de IA
builder.Services.AddHttpClient<IAIService, OpenRouterAIService>();
builder.Services.AddScoped<TextRefactorUseCase>();
builder.Services.AddScoped<ConsequenceAnalyzerUseCase>();
```

## 📦 Paquetes NuGet

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Npgsql" Version="8.0.1" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="MailKit" Version="4.3.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.1.2" />
```

## 🧪 Testing

```bash
# Ejecutar tests
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## 🚀 Publicación

```bash
# Build de producción
dotnet publish -c Release -o ./publish

# Ejecutar publicación
cd publish
dotnet EmailsP.dll
```

## 🔒 Seguridad

- ✅ **Autenticación JWT** con tokens Bearer
- ✅ **Contraseñas hasheadas** con BCrypt
- ✅ **CORS configurado** para el frontend
- ✅ **Validación de datos** en DTOs
- ✅ **Protección de endpoints** con `[Authorize]`

## 📝 Notas Importantes

1. **Credenciales de Gmail**: El archivo `credentials.json` NO debe subirse a git (está en `.gitignore`)
2. **JWT Secret**: Debe tener al menos 32 caracteres
3. **PostgreSQL**: Asegúrate de que el servidor esté corriendo
4. **Primera ejecución**: Gmail solicitará autorización OAuth2

## 🐛 Troubleshooting

### Error: "Failed to determine the https port"
- Normal en desarrollo, el proyecto funciona en HTTP

### Error: "Unable to connect to PostgreSQL"
- Verifica que PostgreSQL esté corriendo
- Revisa la cadena de conexión en `appsettings.Development.json`

### Error: "Gmail authentication failed"
- Verifica que `credentials.json` esté en la ruta correcta
- Elimina el archivo `token.json` y vuelve a autenticarte

### Error 401 en endpoints
- Verifica que el token JWT esté en el header: `Authorization: Bearer {token}`

## 📞 Soporte

Para problemas o preguntas:
- GitHub Issues: [ProyectoEmailsIncognito/issues](https://github.com/SebastianGallegoC/ProyectoEmailsIncognito/issues)
