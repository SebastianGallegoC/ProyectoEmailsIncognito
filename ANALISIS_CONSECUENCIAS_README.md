# ?? Módulo de Análisis de Consecuencias con IA

## ?? Descripción

Sistema de análisis inteligente que predice las consecuencias de enviar mensajes anónimos, especializado en el contexto legal y cultural de **Colombia y Latinoamérica**.

---

## ?? Modelo de IA Usado

**Modelo**: `qwen/qwen-2.5-7b-instruct:free` (OpenRouter)

**Por qué este modelo:**
- ? **Gratuito** (100% free en OpenRouter)
- ? **Excelente para JSON estructurado** (7B parámetros)
- ? Sin rate limits problemáticos
- ? Mejor seguimiento de instrucciones que modelos más pequeños
- ? Muy bueno para análisis contextual

**Separación de modelos:**
- **DeepSeek**: Reformulación de texto (general)
- **Qwen 2.5 7B**: Análisis de consecuencias (especializado)

**Nota**: Si experimentas problemas de JSON, el modelo tiene sistema de fallback automático.

---

## ?? Funcionalidad

Antes de enviar un mensaje anónimo, la IA analiza:

### 1?? **Riesgo Legal** ??
- Posibles problemas legales (difamación, injuria, calumnia)
- Referencias a legislación colombiana/latinoamericana
- Ley 1010 de 2006 (Acoso Laboral Colombia)
- Código Sustantivo del Trabajo
- Realidad práctica vs. teoría legal

### 2?? **Impacto Emocional** ??
- Tono del mensaje (confrontacional, neutral, constructivo)
- Palabras que pueden generar reacción negativa
- Contexto cultural latinoamericano
- Cultura jerárquica y de respeto

### 3?? **Efectividad** ?
- Probabilidad de que genere acción (0-100%)
- Elementos faltantes en el mensaje
- Fortalezas del mensaje
- Recomendaciones específicas para Colombia/Latinoamérica

### 4?? **Riesgo de Represalias** ??
- Posibles consecuencias negativas
- Despidos encubiertos, ambiente hostil
- Consejos de mitigación
- Mecanismos de protección disponibles en Colombia

### 5?? **Evaluación General** ??
- Recomendación: ¿Enviar o no enviar?
- Resumen ejecutivo
- Top 3 prioridades de mejora
- Próximos pasos recomendados

---

## ??? Arquitectura

### Domain Layer
```csharp
Domain/Interfaces/IConsequenceAnalyzerService.cs
```

### Application Layer
```
Application/DTOs/AI/
??? AnalyzeConsequencesRequest.cs
??? ConsequenceAnalysisResponse.cs

Application/Services/
??? ConsequenceAnalyzerUseCase.cs
```

### Infrastructure Layer
```csharp
Infraestructure/AI/ConsequenceAnalyzerService.cs
```

### API Layer
```
POST /api/AI/analyze-consequences
```

---

## ?? Cómo Usar

### Desde la UI (/compose.html)

1. **Escribe tu mensaje** en el campo "Cuerpo"
2. **(Opcional)** Reformúlalo con IA primero
3. **Haz clic en** "?? Analizar Consecuencias"
4. **Espera 10-15 segundos** (análisis completo)
5. **Revisa el análisis detallado**:
   - Riesgo legal
   - Impacto emocional
   - Efectividad
   - Riesgo de represalias
   - Recomendaciones
6. **Decide**: Enviar, modificar o cancelar

### Desde Swagger

**Endpoint**: `POST /api/AI/analyze-consequences`

**Request:**
```json
{
  "text": "Mi jefe me grita todos los días y me amenaza con despedirme",
  "context": "workplace",
  "country": "CO"
}
```

**Contextos soportados:**
- `workplace` (laboral) - por defecto
- `academic` (académico)
- `personal` (personal)

**Países soportados:**
- `CO` - Colombia (por defecto)
- `PE` - Perú
- `EC` - Ecuador
- `VE` - Venezuela
- `BO` - Bolivia
- `PA` - Panamá

**Response:**
```json
{
  "legalRisk": {
    "level": "Medium",
    "description": "Denuncia válida bajo Ley 1010 de 2006...",
    "potentialIssues": ["Requiere evidencia documental"],
    "legalReferences": ["Ley 1010 de 2006 - Acoso Laboral"],
    "practicalReality": "En Colombia, baja tasa de éxito..."
  },
  "emotionalImpact": {
    "level": "High",
    "description": "Lenguaje directo...",
    "detectedTone": "Confrontational",
    "triggerWords": ["amenaza", "grita"],
    "culturalContext": "En Colombia, cultura de respeto jerárquico..."
  },
  "effectiveness": {
    "probabilityOfAction": 35,
    "reasoning": "Probabilidad media-baja...",
    "missingElements": ["Fechas específicas", "Evidencia documental"],
    "strengthPoints": ["Describe conducta específica"],
    "localRecommendations": "Documentar cada incidente..."
  },
  "backlash": {
    "level": "High",
    "potentialConsequences": ["Despido sin justa causa", "Ambiente hostil"],
    "mitigationAdvice": "NO enviar desde correo corporativo...",
    "localProtections": "Ministerio del Trabajo: 018000 112518"
  },
  "overall": {
    "recommendSending": false,
    "summary": "NO recomendado. Necesita más evidencia...",
    "topPriorities": [
      "Documentar incidentes específicos",
      "Consultar abogado laboral",
      "Intentar solución interna primero"
    ],
    "nextSteps": "Descargar app de grabación, anotar incidentes..."
  }
}
```

---

## ???? Especialización en Colombia y Latinoamérica

### Legislación considerada:
- ? Código Sustantivo del Trabajo de Colombia
- ? Ley 1010 de 2006 (Acoso Laboral)
- ? Constitución Política (Art. 25, 53)
- ? Sentencias de la Corte Constitucional
- ? Legislación laboral de Perú, Ecuador, etc.

### Contexto cultural considerado:
- ? Jerarquías organizacionales marcadas
- ? Importancia del respeto y formalidad
- ? Temor a represalias laborales
- ? Informalidad laboral prevalente
- ? Protección legal limitada en práctica
- ? Debilidad institucional

### Realidad práctica:
- ? Reconoce brecha entre teoría legal y práctica
- ? Considera recursos limitados de instituciones
- ? Advierte sobre represalias comunes
- ? Recomienda documentación exhaustiva
- ? Sugiere recursos locales (Ministerio, Defensorías)

---

## ?? Configuración

### appsettings.Development.json
```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-tu-key-aqui",
    "Model": "deepseek/deepseek-chat",
    "AnalyzerModel": "qwen/qwen-2.5-7b-instruct:free"
  }
}
```

### Dependencias en Program.cs
```csharp
// AI - Consequence Analysis (Llama 3.2 3B)
builder.Services.AddHttpClient<IConsequenceAnalyzerService, ConsequenceAnalyzerService>();
builder.Services.AddScoped<ConsequenceAnalyzerUseCase>();
```

---

## ??? Sistema de Fallback

Si la IA falla temporalmente (error de red, rate limit, etc.), el sistema provee un **análisis básico rule-based**:

```
?? Análisis completo temporalmente no disponible. 
Por precaución, recomendamos esperar y reintentar.

Análisis básico:
- Detecta lenguaje ofensivo
- Detecta falta de evidencia
- Detecta falta de solución propuesta
- Recomienda consultar con experto legal
```

---

## ?? Ejemplo de Uso Real

**Texto ingresado:**
```
Mi jefe me grita en las reuniones delante de todos y me hace quedar mal. 
Ya no aguanto más esto, es una humillación constante.
```

**Análisis generado:**

?? **Riesgo Legal**: MEDIO  
? Posible acoso laboral según Ley 1010/2006. Requiere evidencia testimonial.

?? **Impacto Emocional**: ALTO  
? Lenguaje emocional puede generar defensividad. Tono: Confrontacional

? **Efectividad**: 30%  
? Falta evidencia concreta (fechas, testigos, intentos previos de solución)

?? **Riesgo de Represalias**: ALTO  
? Despido sin justa causa, ambiente más hostil

**Recomendación**: ? NO ENVIAR AÚN

**Prioridades:**
1. Documentar fechas y horas específicas
2. Identificar testigos
3. Intentar solución interna primero (requisito legal)

**Próximos pasos:**
- Anotar cada incidente en diario personal
- Consultar abogado laboral (asesorías gratuitas disponibles)
- Conocer procedimiento interno de la empresa

---

## ?? Seguridad y Privacidad

- ? Requiere autenticación JWT
- ? Los textos no se almacenan en BD
- ? Solo se envían a la IA para análisis
- ? Respuestas no se guardan
- ? Validación de longitud (máx 5000 caracteres)

---

## ?? Limitaciones

1. **No es asesoría legal profesional** - Siempre consultar con abogado
2. **Análisis basado en contexto general** - Cada caso es único
3. **Depende de calidad del modelo** - Puede haber errores
4. **Contexto limitado** - No conoce todos los detalles del caso

**Siempre recomendamos consultar with experto legal before de tomar acciones importantes.**

---

## ?? Troubleshooting

### Error: "El texto es muy largo"
- **Límite**: 5000 caracteres
- **Solución**: Resumir el mensaje principal

### Error: "Error communicating with AI service"
- **Causa**: OpenRouter temporalmente no disponible
- **Solución**: Esperar 1-2 minutos y reintentar
- **Fallback**: Se mostrará análisis básico rule-based

### Análisis parece genérico
- **Causa**: Texto muy corto o vago
- **Solución**: Proporcionar más contexto y detalles específicos

---

## ?? Referencias

- [Ley 1010 de 2006 - Acoso Laboral Colombia](http://www.secretariasenado.gov.co/senado/basedoc/ley_1010_2006.html)
- [Código Sustantivo del Trabajo Colombia](http://www.secretariasenado.gov.co/senado/basedoc/codigo_sustantivo_trabajo.html)
- [Ministerio del Trabajo Colombia](https://www.mintrabajo.gov.co/)
- Línea gratuita Ministerio: **018000 112518**

---

## ?? Valor Diferencial

Este módulo convierte **EmailsP** de un simple "enviador de emails anónimos" a una **plataforma inteligente de comunicación responsable** que:

? Protege al usuario de consecuencias legales  
? Educa sobre derechos laborales  
? Recomienda mejores prácticas  
? Aumenta efectividad de denuncias  
? Reduce riesgo de represalias  
? Empodera al denunciante con información  

**No solo enviamos mensajes, ayudamos a que sean efectivos y seguros.** ??
