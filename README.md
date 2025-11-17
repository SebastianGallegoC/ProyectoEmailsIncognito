# 📧 EmailsP Incógnito - Proyecto Completo

Sistema de envío de correos electrónicos anónimos con funcionalidades de IA para análisis y reformulación de textos.

## 🏗️ Arquitectura del Proyecto

```
ProyectoEmailsIncognito/
├── EmailsP/                 # Backend - API .NET
│   ├── Domain/             # Entidades y contratos
│   ├── Application/        # Casos de uso y DTOs
│   ├── Infraestructure/    # Implementaciones
│   └── EmailsP/            # API Web (Controllers)
│
└── emailsp-client/         # Frontend - Vue.js 3
    ├── src/
    │   ├── views/          # Páginas
    │   ├── components/     # Componentes
    │   ├── services/       # API clients
    │   └── stores/         # Estado global
    └── public/
```

## ⚙️ Tecnologías

### Backend
- **.NET 8** - Framework web
- **Clean Architecture** - Separación de responsabilidades
- **JWT** - Autenticación
- **MailKit** - Envío de emails
- **PostgreSQL** - Base de datos
- **OpenRouter API** - Integración con IA

### Frontend
- **Vue 3** - Framework progresivo
- **TypeScript** - Tipado estático
- **Tailwind CSS** - Estilos
- **Pinia** - Gestión de estado
- **Axios** - Cliente HTTP

## 🚀 Instalación y Ejecución

### 1️⃣ Backend (.NET)

```bash
cd EmailsP

# Restaurar dependencias (automático al compilar)
dotnet restore

# Configurar variables de entorno
# Copiar .env.example y configurar:
# - Cadena de conexión PostgreSQL
# - JWT Secret
# - Credenciales de Gmail
# - API Key de OpenRouter

# Ejecutar el proyecto
dotnet run --project EmailsP/EmailsP.csproj

# El backend estará en: http://localhost:5162
# Swagger UI: http://localhost:5162/swagger
```

### 2️⃣ Frontend (Vue.js)

```bash
cd emailsp-client

# Instalar dependencias (si es necesario)
npm install

# Ejecutar en modo desarrollo
npm run dev

# El frontend estará en: http://localhost:5173
```

## 📋 Configuración Requerida

### Backend - Variables de Entorno

Crear archivo `EmailsP/EmailsP/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=emailsp;Username=postgres;Password=tu_password"
  },
  "JwtSettings": {
    "Secret": "tu-clave-secreta-jwt-minimo-32-caracteres",
    "Issuer": "EmailsP",
    "Audience": "EmailsPClient",
    "ExpirationHours": 24
  },
  "OpenRouter": {
    "ApiKey": "tu-api-key-openrouter",
    "BaseUrl": "https://openrouter.ai/api/v1"
  },
  "Gmail": {
    "FromEmail": "tu-email@gmail.com",
    "FromName": "EmailsP Anónimo"
  }
}
```

### Gmail - Configuración OAuth2

1. Crear proyecto en [Google Cloud Console](https://console.cloud.google.com/)
2. Habilitar Gmail API
3. Crear credenciales OAuth 2.0
4. Descargar `credentials.json` → `EmailsP/Infraestructure/credentials/`

### PostgreSQL - Base de Datos

```sql
-- Crear base de datos
CREATE DATABASE emailsp;

-- Tabla de usuarios
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de contactos
CREATE TABLE contacts (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES usuarios(id),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    is_favorite BOOLEAN DEFAULT FALSE,
    is_blocked BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Usuario por defecto
INSERT INTO usuarios (username, password_hash, email) 
VALUES ('admin', '$2a$11$hashed_password', 'admin@emailsp.com');
```

## 🔑 Credenciales por Defecto

```
Usuario: admin
Contraseña: admin123
```

## 📚 Documentación Adicional

- [README Backend](./EmailsP/README.md) - Detalles de la API
- [README Frontend](./emailsp-client/README_CLIENTE.md) - Detalles del cliente

## ✨ Funcionalidades Principales

### 🔐 Autenticación
- Login/Registro con JWT
- Protección de rutas
- Sesión persistente

### 📨 Envío de Emails
- Correos anónimos
- Múltiples destinatarios
- Adjuntos hasta 25MB
- Templates HTML profesionales

### 👥 Gestión de Contactos
- CRUD completo
- Favoritos y bloqueados
- Búsqueda y filtros
- Paginación

### 🤖 Inteligencia Artificial
- **Reformulador**: Convierte texto informal a formal
- **Analizador**: Evalúa riesgos legales, emocionales y efectividad
- Powered by DeepSeek R1 (OpenRouter)

## 🛠️ Scripts Útiles

### Backend
```bash
# Compilar
dotnet build

# Tests
dotnet test

# Publicar
dotnet publish -c Release
```

### Frontend
```bash
# Desarrollo
npm run dev

# Build producción
npm run build

# Preview build
npm run preview

# Tests unitarios
npm run test:unit

# Tests E2E
npm run test:e2e
```

## 📦 Dependencias Principales

### Backend
- Microsoft.AspNetCore.Authentication.JwtBearer
- Npgsql.EntityFrameworkCore.PostgreSQL
- MailKit
- BCrypt.Net-Next
- Swashbuckle.AspNetCore

### Frontend
- vue@3
- vue-router
- pinia
- axios
- tailwindcss

## 🤝 Contribución

Este es un proyecto académico. Para contribuir:

1. Fork el repositorio
2. Crea una rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -m 'feat: agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

## 📄 Licencia

Proyecto académico - FESC (Fundación de Estudios Superiores Comfanorte)

## 👥 Autores

- **Sebastian Gallego C.** - [SebastianGallegoC](https://github.com/SebastianGallegoC)
- **Waldo** - [waldooCreator](https://github.com/waldooCreator)

## 🔗 Enlaces

- **Repositorio**: [ProyectoEmailsIncognito](https://github.com/SebastianGallegoC/ProyectoEmailsIncognito)
- **API Swagger**: http://localhost:5162/swagger
- **Frontend**: http://localhost:5173
