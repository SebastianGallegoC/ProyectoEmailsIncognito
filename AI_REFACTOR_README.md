# AI Text Refactoring Feature

## ?? Descripción

Esta funcionalidad permite refactorizar textos para hacerlos más formales y profesionales usando IA, sin alterar su significado original. Utiliza **DeepSeek Chat** (modelo gratuito de OpenRouter).

## ?? Modelo Actual

- **Modelo**: `deepseek/deepseek-chat`
- **Proveedor**: DeepSeek via OpenRouter
- **Costo**: Gratuito
- **Límite**: Muy generoso, sin rate limits estrictos
- **Calidad**: ????? (comparable a GPT-4)

> ?? **Nota**: Puedes cambiar el modelo en `appsettings.Development.json`. Ver `MODELOS_IA_DISPONIBLES.md` para otras opciones.

## ??? Arquitectura (DDD + Clean Architecture)

### 1?? Domain Layer
- **Interfaz**: `Domain/Interfaces/IAIService.cs`
  - Define el contrato para servicios de IA

### 2?? Application Layer
- **DTOs**:
  - `Application/DTOs/AI/RefactorRequest.cs` - Request DTO
  - `Application/DTOs/AI/RefactorResponse.cs` - Response DTO
- **Use Case**: `Application/Services/TextRefactorUseCase.cs`
  - Orquesta la lógica de negocio para refactorización

### 3?? Infrastructure Layer
- **Implementación**: `Infraestructure/AI/OpenRouterAIService.cs`
  - Implementa `IAIService`
  - Se comunica con OpenRouter API
  - Maneja autenticación y errores

### 4?? API Layer
- **Controller**: `EmailsP/Controllers/AIController.cs`
  - Endpoint: `POST /api/AI/refactor`
  - Requiere autenticación JWT
  - Maneja validación y errores

## ?? Configuración

### **Opción 1: Usando appsettings.Development.json (RECOMENDADO)**

Edita `EmailsP/appsettings.Development.json` y agrega tu API key:

```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-TU-API-KEY-AQUI",
    "Model": "deepseek/deepseek-chat"
  }
}
```

? **Esta es la forma más simple y no requiere reiniciar Visual Studio**

### **Opción 2: Variables de Entorno**

Puedes usar variables de entorno si prefieres:

```bash
# Windows (PowerShell) - Temporal (solo sesión actual)
$env:OPENROUTER_API_KEY = "tu-api-key-aqui"

# Windows (PowerShell) - Permanente
[System.Environment]::SetEnvironmentVariable('OPENROUTER_API_KEY', 'tu-api-key-aqui', 'User')

# Windows (CMD)
set OPENROUTER_API_KEY=tu-api-key-aqui

# Linux/Mac
export OPENROUTER_API_KEY=tu-api-key-aqui
```

?? **Importante**: Si usas variables de entorno, debes **reiniciar Visual Studio** después de configurarlas.

### Obtener API Key

1. Ve a [OpenRouter](https://openrouter.ai/)
2. Crea una cuenta (gratis)
3. Genera una API Key (gratis)
4. Copia la key que comienza con `sk-or-v1-...`
5. Pégala en `appsettings.Development.json` o configúrala como variable de entorno

## ?? Uso

### Desde la UI (compose.html)

1. **Configura tu API key** (ver sección anterior)
2. **Reinicia la aplicación** (Ctrl+F5 en Visual Studio)
3. Escribe tu texto en el campo "Cuerpo"
4. Haz clic en el botón **"? Reformular con IA"**
5. Espera unos segundos (el modelo puede tardar)
6. El texto reformulado aparecerá en el mismo campo

### Desde API (cURL)

```bash
curl -X POST https://localhost:7xxx/api/AI/refactor \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "hola como estas quiero pedirte un favor"
  }'
```

**Respuesta:**
```json
{
  "formalText": "Estimado/a,\n\nEspero que se encuentre bien. Me dirijo a usted para solicitar un favor..."
}
```

### Desde Swagger

1. Navega a `/swagger`
2. Autentícate con tu JWT
3. Encuentra el endpoint `POST /api/AI/refactor`
4. Prueba con un texto de ejemplo

## ?? Seguridad

- ? Requiere autenticación JWT
- ? Valida que el texto no esté vacío
- ? API Key almacenada en appsettings (solo para desarrollo)
- ? En producción usa variables de entorno o Azure Key Vault
- ? Manejo robusto de errores

## ?? Dependencias Inyectadas

En `Program.cs`:

```csharp
// AI - Text Refactoring
builder.Services.AddHttpClient<IAIService, OpenRouterAIService>();
builder.Services.AddScoped<TextRefactorUseCase>();
```

## ?? Testing

### Prueba Manual

1. **Configura la API key** en `appsettings.Development.json`:
   ```json
   {
     "OpenRouter": {
       "ApiKey": "sk-or-v1-tu-key-aqui",
       "Model": "deepseek/deepseek-chat"
     }
   }
   ```
2. **Reinicia la aplicación** (Ctrl+F5)
3. Obtén un JWT desde `/api/Auth/login`
4. Ve a `/compose.html`
5. Escribe un texto informal
6. Presiona "? Reformular con IA"
7. Verifica que el texto se reformule correctamente

### Prueba desde Swagger

1. Ve a `/swagger`
2. Autentícate con JWT
3. Ejecuta `POST /api/AI/refactor` con:
```json
{
  "text": "hey man whats up need ur help asap"
}
```
4. Deberías recibir un texto más formal

## ?? Troubleshooting

### ? Error: "OpenRouter API key not found"

**Solución:**
1. Abre `EmailsP/appsettings.Development.json`
2. Agrega tu API key:
   ```json
   {
     "OpenRouter": {
       "ApiKey": "sk-or-v1-TU-KEY-AQUI",
       "Model": "deepseek/deepseek-chat"
     }
   }
   ```
3. Reinicia la aplicación (Ctrl+F5)

### ? Error 502: Bad Gateway

**Causas posibles:**
- ? Verifica tu conexión a internet
- ? Verifica que la API Key sea válida
- ? Verifica que no hayas excedido el límite gratuito de OpenRouter

### ? Error 429: Rate Limit (Gemini)

**Solución:**
- Cambia a DeepSeek que no tiene rate limits estrictos:
```json
{
  "OpenRouter": {
    "Model": "deepseek/deepseek-chat"
  }
}
```

### ? Error 401: Unauthorized

**Causas posibles:**
- ? Verifica que tu JWT sea válido (no expiró)
- ? Genera un nuevo token desde `/api/Auth/login`

### ? La API key no se lee correctamente

**Solución:**
1. Verifica que el archivo sea `appsettings.Development.json` (con mayúscula en Development)
2. Verifica que la estructura JSON sea correcta (sin comas extras)
3. Reinicia Visual Studio completamente
4. Verifica que estés ejecutando en modo Development

## ?? Ejemplo de Transformación

**Entrada:**
```
hola como estas? necesito que me ayudes con un tema urgente
```

**Salida:**
```
Estimado/a,

Espero que se encuentre bien. Me dirijo a usted para solicitar su asistencia con un asunto de carácter urgente.

Quedo a la espera de su pronta respuesta.

Saludos cordiales.
```

## ?? Próximas Mejoras

- [ ] Cache de respuestas para textos similares
- [ ] Soporte para múltiples modelos de IA
- [ ] Configuración de tono (formal, casual, técnico)
- [ ] Límite de caracteres configurable
- [ ] Métricas de uso y performance

## ?? Producción

Para producción, **NO coloques la API key en appsettings.json**. Usa:

1. **Variables de entorno del servidor**
2. **Azure Key Vault**
3. **AWS Secrets Manager**
4. **Kubernetes Secrets**

El código ya soporta leer desde variables de entorno automáticamente.

## ?? ¿Por qué DeepSeek?

- ? **Gratuito** sin límites estrictos
- ? **Calidad comparable a GPT-4**
- ? **Muy rápido** (optimizado)
- ? **Sin rate limits del proveedor**
- ? **Alta disponibilidad** (99%+)
- ? **Excelente para reformulación de textos**

Ver `MODELOS_IA_DISPONIBLES.md` para comparativa completa con otros modelos.
