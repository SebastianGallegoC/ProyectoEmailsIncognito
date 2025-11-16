# ?? Solución al Error 502 - AI Service Error

## ?? ¿Qué significa el error?

El error 502 (Bad Gateway) indica que la aplicación no puede comunicarse correctamente con la API de OpenRouter.

## ? Pasos para Solucionar

### 1?? Verifica que la API Key esté configurada

Abre `EmailsP/appsettings.Development.json` y verifica que tu API key esté correctamente configurada:

```json
{
  "OpenRouter": {
    "ApiKey": "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"
  }
}
```

### 2?? Reinicia la aplicación

Después de configurar la API key, **DEBES reiniciar** la aplicación:

- Presiona **Ctrl+F5** en Visual Studio, o
- Detén la aplicación (Shift+F5) y vuelve a ejecutarla (F5)

### 3?? Verifica tu conexión a Internet

OpenRouter es un servicio en la nube. Asegúrate de tener conexión estable a internet.

### 4?? Verifica que tu API Key sea válida

1. Ve a https://openrouter.ai/
2. Inicia sesión
3. Ve a "API Keys"
4. Verifica que tu key esté activa y no haya expirado
5. Verifica que tengas créditos disponibles (el modelo gratuito tiene límites)

### 5?? Prueba con un texto simple primero

Comienza con un texto corto para verificar que funciona:

```json
{
  "text": "hola como estas"
}
```

### 6?? Revisa los logs de Visual Studio

Ahora el código muestra **detalles del error en modo Development**:

1. Ve a la ventana **Output** en Visual Studio
2. Busca mensajes de error detallados
3. Deberías ver el error exacto de OpenRouter

## ?? Errores Comunes

### Error: "Invalid API Key"

**Solución:**
- Tu API key no es válida o está mal copiada
- Ve a OpenRouter y genera una nueva
- Asegúrate de copiar toda la key completa (empieza con `sk-or-v1-...`)

### Error: "Rate limit exceeded"

**Solución:**
- Has excedido el límite gratuito
- Espera unos minutos antes de volver a intentar
- Considera upgradearte a un plan de pago si lo necesitas frecuentemente

### Error: "Model not found"

**Solución:**
- El modelo gratuito puede no estar disponible temporalmente
- OpenRouter puede haber cambiado los modelos disponibles
- Verifica en https://openrouter.ai/models qué modelos están disponibles

### Error: "Timeout"

**Solución:**
- El servicio de OpenRouter está tardando mucho
- Prueba con un texto más corto
- Intenta de nuevo en unos minutos

## ?? Cómo Probar Paso a Paso

### Opción 1: Desde Swagger (Recomendado para diagnóstico)

1. Ve a `/swagger`
2. Autentica con tu JWT
3. Busca `POST /api/AI/refactor`
4. Prueba con este JSON:
   ```json
   {
     "text": "hola necesito ayuda"
   }
   ```
5. Si falla, verás el **error detallado** en la respuesta

### Opción 2: Desde compose.html

1. Ve a `/compose.html`
2. Escribe un texto corto
3. Haz clic en "? Reformular con IA"
4. Abre la **consola del navegador** (F12) para ver errores

### Opción 3: Con cURL (Terminal)

```bash
curl -X POST "https://localhost:7xxx/api/AI/refactor" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"text":"hola como estas"}'
```

## ?? Checklist de Verificación

Marca cada paso que hayas completado:

- [ ] ? API key configurada en `appsettings.Development.json`
- [ ] ? Aplicación reiniciada después de configurar
- [ ] ? Conexión a internet estable
- [ ] ? API key validada en OpenRouter.ai
- [ ] ? Logs revisados en Visual Studio
- [ ] ? Probado con texto simple primero
- [ ] ? Sin firewall/antivirus bloqueando la conexión

## ?? Verificación Avanzada

Si todo lo anterior está bien, prueba esto:

### Verifica manualmente con cURL directo a OpenRouter

```bash
curl https://openrouter.ai/api/v1/chat/completions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer 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" \
  -d '{
    "model": "meta-llama/llama-3.1-8b-instruct:free",
    "messages": [
      {"role": "user", "content": "Say hello"}
    ]
  }'
```

Si esto falla, el problema es con tu API key o con OpenRouter, no con tu aplicación.

## ?? Próximo Paso

Después de seguir estos pasos:

1. **Reinicia la aplicación** (importante!)
2. Prueba de nuevo desde `/swagger`
3. Revisa los **logs en la ventana Output** para ver el error exacto

El error ahora mostrará detalles completos en modo Development, así que podrás ver exactamente qué está fallando.

## ?? ¿Todavía no funciona?

Si después de todo esto sigue fallando:

1. Copia el **error completo** de la ventana Output de Visual Studio
2. Verifica el estado de OpenRouter en: https://status.openrouter.ai/
3. Considera usar un modelo diferente (consulta modelos disponibles en OpenRouter)
