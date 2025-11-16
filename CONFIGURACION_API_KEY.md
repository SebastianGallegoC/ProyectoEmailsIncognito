# ?? Configuración de la API Key de OpenRouter

## ?? Solución Rápida (RECOMENDADA)

### Paso 1: Obtén tu API Key

1. Ve a: https://openrouter.ai/
2. Crea una cuenta (es gratis)
3. En tu dashboard, ve a "API Keys"
4. Crea una nueva API key
5. Copia la key que comienza con `sk-or-v1-...`

### Paso 2: Configura la API Key

Edita el archivo **`EmailsP/appsettings.Development.json`**:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "OpenRouter": {
    "ApiKey": "sk-or-v1-PEGA-TU-KEY-AQUI"
  }
}
```

### Paso 3: Reinicia la aplicación

Presiona **Ctrl+F5** en Visual Studio para reiniciar la aplicación.

### Paso 4: Prueba

1. Ve a `/compose.html`
2. Escribe un texto en el campo "Cuerpo"
3. Haz clic en **"? Reformular con IA"**
4. ¡Listo! ??

---

## ?? Método Alternativo: Variables de Entorno

Si prefieres usar variables de entorno en lugar de appsettings.json:

### Windows PowerShell (Permanente)
```powershell
[System.Environment]::SetEnvironmentVariable('OPENROUTER_API_KEY', 'sk-or-v1-tu-key', 'User')
```

Luego **reinicia Visual Studio completamente**.

### Windows PowerShell (Temporal - solo sesión actual)
```powershell
$env:OPENROUTER_API_KEY = "sk-or-v1-tu-key"
```

Luego reinicia la aplicación (Ctrl+F5).

---

## ?? Importante

- ? **NO** subas tu API key al repositorio de Git
- ? Usa `appsettings.Development.json` solo para desarrollo local
- ? En producción, usa variables de entorno del servidor o servicios como Azure Key Vault

---

## ?? Solución de Problemas

### Error: "OpenRouter API key not found"

**Causa:** La API key no está configurada.

**Solución:**
1. Verifica que editaste el archivo correcto: `EmailsP/appsettings.Development.json`
2. Verifica que la estructura JSON sea correcta (sin comas extras)
3. Verifica que la API key esté entre comillas
4. Reinicia la aplicación (Ctrl+F5)

### La API key no se lee

**Solución:**
1. Cierra Visual Studio completamente
2. Abre de nuevo el proyecto
3. Verifica que estés ejecutando en modo **Development** (no Release)
4. Intenta con variables de entorno como alternativa

### Error 502: Bad Gateway

**Causa:** Problema comunicándose con OpenRouter.

**Solución:**
1. Verifica tu conexión a internet
2. Verifica que tu API key sea válida (ve a openrouter.ai y verifica)
3. Verifica que no hayas excedido el límite gratuito

---

## ?? Soporte

Si sigues teniendo problemas, verifica:
- ? Que la API key sea válida
- ? Que tengas conexión a internet
- ? Que hayas reiniciado la aplicación
- ? Los logs en la consola de Visual Studio (Output window)
