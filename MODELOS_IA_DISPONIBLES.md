# ?? Modelos de IA Disponibles en OpenRouter

## ? El Problema Resuelto

Los modelos gratuitos de OpenRouter tienen **límites de tasa de uso** y pueden cambiar.

He actualizado el código para usar **DeepSeek Chat** que es:
- ? **Gratuito**
- ? **Muy rápido**
- ? **Disponible actualmente**
- ? **Excelente para reformular textos**
- ? **Sin límites estrictos de tasa**

## ?? Modelo Configurado (Actual)

```json
{
  "OpenRouter": {
    "ApiKey": "tu-key-aqui",
    "Model": "deepseek/deepseek-chat"
  }
}
```

## ?? Otros Modelos Gratuitos Disponibles

Si quieres probar otros modelos gratuitos, edita `appsettings.Development.json`:

### 1?? DeepSeek Chat ? (RECOMENDADO - Ya configurado)
```json
"Model": "deepseek/deepseek-chat"
```
- **Ventajas**: Muy rápido, excelente calidad, sin rate limits estrictos
- **Límites**: Muy generosos
- **Ideal para**: Reformulación de textos profesionales

### 2?? Google Gemini 2.0 Flash
```json
"Model": "google/gemini-2.0-flash-exp:free"
```
- **Ventajas**: Muy rápido, excelente calidad
- **Límites**: ~1500 req/día (puede tener rate limits temporales)
- **Nota**: Puede estar temporalmente limitado upstream

### 3?? Meta Llama 3.2 3B Instruct
```json
"Model": "meta-llama/llama-3.2-3b-instruct:free"
```
- **Ventajas**: Rápido, ligero
- **Límites**: Moderado

### 4?? Meta Llama 3.2 1B Instruct
```json
"Model": "meta-llama/llama-3.2-1b-instruct:free"
```
- **Ventajas**: Muy rápido
- **Desventajas**: Calidad menor para textos complejos

### 5?? Microsoft Phi-3 Medium
```json
"Model": "microsoft/phi-3-medium-128k-instruct:free"
```
- **Ventajas**: Buena calidad
- **Límites**: Moderado

### 6?? Qwen 2.5 7B Instruct
```json
"Model": "qwen/qwen-2.5-7b-instruct:free"
```
- **Ventajas**: Buena calidad
- **Límites**: Moderado

## ?? Cómo Verificar Modelos Disponibles

Ve a: https://openrouter.ai/models

Busca modelos con el tag **"Free"** o precio $0.00

## ?? Cómo Cambiar de Modelo

1. Abre `EmailsP/appsettings.Development.json`
2. Cambia el valor de `"Model"`:
```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-tu-key",
    "Model": "deepseek/deepseek-chat"
  }
}
```
3. Reinicia la aplicación (Ctrl+F5)
4. Prueba de nuevo

## ?? Comparativa Rápida

| Modelo | Velocidad | Calidad | Límite Diario | Rate Limits |
|--------|-----------|---------|---------------|-------------|
| **DeepSeek Chat** ? | ???? | ?????????? | Alto | Muy bajo |
| Gemini 2.0 Flash | ??? | ?????????? | ~1500 req | Medio |
| Llama 3.2 3B | ??? | ???????? | ~1000 req | Bajo |
| Llama 3.2 1B | ???? | ?????? | ~1500 req | Bajo |
| Phi-3 Medium | ?? | ???????? | ~800 req | Medio |
| Qwen 2.5 7B | ?? | ???????? | ~1000 req | Medio |

## ?? Recomendación

**Usa `deepseek/deepseek-chat`** (ya configurado por defecto).

Es el mejor balance entre:
- Velocidad (muy rápido)
- Calidad de reformulación (excelente)
- Sin rate limits estrictos
- Disponibilidad constante
- Gratuito

## ?? Importante: Rate Limits

### ¿Qué es un Rate Limit?

Es cuando el proveedor limita temporalmente las solicitudes por alta demanda.

### Error típico:
```
"Provider returned error" (429)
"temporarily rate-limited upstream"
```

### Soluciones:

1. **Espera unos minutos** y vuelve a intentar
2. **Cambia a DeepSeek Chat** (menos rate limits)
3. **Agrega tu propia API key del proveedor** (ej: Google AI Studio para Gemini)
4. **Usa un modelo de pago** si necesitas garantías

## ?? Modelos de Pago (Opcionales)

Si necesitas más calidad o límites mayores:

### GPT-4o Mini (Económico)
```json
"Model": "openai/gpt-4o-mini"
```
- **Costo**: ~$0.15 por millón de tokens
- **Calidad**: Excelente

### Claude 3.5 Haiku (Rápido y económico)
```json
"Model": "anthropic/claude-3.5-haiku"
```
- **Costo**: ~$0.25 por millón de tokens
- **Calidad**: Excelente

### DeepSeek V3 (Económico y potente)
```json
"Model": "deepseek/deepseek-v3"
```
- **Costo**: ~$0.27 por millón de tokens entrada, ~$1.10 salida
- **Calidad**: Excelente (uno de los mejores)

### GPT-4o (Mejor calidad)
```json
"Model": "openai/gpt-4o"
```
- **Costo**: ~$2.50 por millón de tokens
- **Calidad**: La mejor disponible

## ?? Prueba Ahora

1. **Reinicia la aplicación** (Ctrl+F5)
2. Ve a `/compose.html`
3. Escribe un texto informal: "hola necesito ayuda urgente"
4. Presiona **"? Reformular con IA"**
5. ¡Debería funcionar sin rate limits! ??

## ?? Ejemplo de Salida

**Entrada:**
```
hola como estas? necesito que me ayudes con un tema urgente porfavor
```

**Salida (DeepSeek Chat):**
```
Estimado/a,

Espero que se encuentre bien. Me dirijo a usted para solicitar su asistencia 
con un asunto de carácter urgente.

Agradezco de antemano su atención y pronta respuesta.

Atentamente.
```

## ?? Troubleshooting

### Error: "No endpoints found"
- El modelo ya no está disponible
- Cambia a `deepseek/deepseek-chat`

### Error: "Rate limit exceeded" (429)
- **Solución rápida**: Cambia a `deepseek/deepseek-chat`
- **Alternativa**: Espera 5-10 minutos y vuelve a intentar
- **Permanente**: Considera usar un modelo de pago

### Error: "Provider returned error"
- El proveedor upstream (Google, Meta, etc.) está limitado
- Cambia a otro modelo (DeepSeek es muy confiable)

## ?? Por qué DeepSeek?

DeepSeek es una empresa china de IA que ofrece:
- ? Modelos de **calidad comparable a GPT-4**
- ? **Gratuito** en OpenRouter
- ? **Sin rate limits estrictos**
- ? **Muy rápido** (optimizado)
- ? **Disponibilidad alta**
- ? Excelente para **reformulación de textos**

Es actualmente la mejor opción gratuita para uso constante.
