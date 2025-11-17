# ?? Solución al Error 400 - JSON Binding Failed

## ?? Error Actual

```json
{
  "errors": {
    "request": ["The request field is required."],
    "$.text": ["'0x0A' is invalid within a JSON string"]
  }
}
```

## ?? Diagnóstico

Hay **DOS problemas simultáneos**:

1. **Binding falla** ? `"request field is required"`
2. **JSON mal formado** ? `'0x0A' is invalid`

## ? Solución Implementada

### 1. Frontend mejorado (compose.html)

```javascript
// Normalizar saltos de línea
bodyText = bodyText.replace(/\r\n/g, '\n').replace(/\r/g, '\n');

// Agregar charset explícito
headers: {
  "Content-Type": "application/json; charset=utf-8"
}
```

### 2. DTO sin validaciones estrictas

```csharp
public class AnalyzeConsequencesRequest
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
    // SIN [Required] para evitar conflictos con binding
}
```

### 3. Validación manual en Controller

```csharp
if (request == null)
    return BadRequest(new { error = "Request body could not be parsed" });

if (string.IsNullOrWhiteSpace(request.Text))
    return BadRequest(new { error = "Text is required" });
```

### 4. Endpoint de prueba

Nuevo endpoint para verificar binding:
```
POST /api/AI/test-binding
```

---

## ?? Pasos de Troubleshooting

### Paso 1: Reinicia la aplicación
```
Ctrl + F5
```

### Paso 2: Prueba el endpoint de test

**En Swagger o Postman:**
```json
POST /api/AI/test-binding
{
  "text": "Texto de prueba simple",
  "context": "workplace",
  "country": "CO"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "success": true,
  "receivedText": "Texto de prueba simple",
  "textLength": 22,
  "context": "workplace",
  "country": "CO",
  "message": "Binding successful!"
}
```

? **Si funciona**: El binding está OK, el problema es solo con textos largos  
? **Si NO funciona**: Hay problema de configuración JSON en .NET

### Paso 3: Probar con texto SIN saltos de línea

```json
{
  "text": "Este es un texto de prueba sin saltos de linea para verificar que el binding funciona correctamente",
  "context": "workplace",
  "country": "CO"
}
```

### Paso 4: Probar con texto CON saltos de línea

**IMPORTANTE**: En Swagger/Postman, el JSON se escapa automáticamente.

```json
{
  "text": "Línea 1\nLínea 2\nLínea 3",
  "context": "workplace",
  "country": "CO"
}
```

### Paso 5: Ver logs en Output window

Visual Studio ? Output ? Buscar:
```
Analyze request received. Request null: False
Request received - Text length: XXX
```

---

## ?? Solución Alternativa: Usar Base64

Si el problema persiste con saltos de línea, podemos encodear en Base64:

### Frontend:
```javascript
const encodedText = btoa(unescape(encodeURIComponent(bodyText)));
body: JSON.stringify({
  text: encodedText,
  isBase64: true
})
```

### Backend:
```csharp
if (request.IsBase64)
{
    var bytes = Convert.FromBase64String(request.Text);
    request.Text = System.Text.Encoding.UTF8.GetString(bytes);
}
```

---

## ?? Checklist de Verificación

- [ ] ? Aplicación reiniciada
- [ ] ? `/api/AI/test-binding` funciona (200 OK)
- [ ] ? Texto simple funciona
- [ ] ? Texto con `\n` funciona
- [ ] ? Logs muestran request recibido
- [ ] ? No hay errores en Output window

---

## ?? Debug Avanzado

### Ver el JSON exacto que se envía

En el navegador (F12 ? Network ? Payload):
```json
{
  "text": "texto\\ncon\\nsaltos",  // \\n está escapado correctamente
  "context": "workplace",
  "country": "CO"
}
```

### Ver el JSON raw en backend

Agregar middleware temporal en Program.cs:
```csharp
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/AI/analyze"))
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        Console.WriteLine($"RAW JSON: {body}");
        context.Request.Body.Position = 0;
    }
    await next();
});
```

---

## ?? Si Nada Funciona

### Opción 1: Simplificar el DTO

```csharp
public class AnalyzeConsequencesRequest
{
    public string text { get; set; }  // lowercase, sin atributos
    public string context { get; set; } = "workplace";
    public string country { get; set; } = "CO";
}
```

### Opción 2: Usar FormData en lugar de JSON

```javascript
const fd = new FormData();
fd.append("text", bodyText);
fd.append("context", "workplace");
fd.append("country", "CO");

fetch("/api/AI/analyze-consequences", {
  method: "POST",
  body: fd  // Sin Content-Type, se pone automáticamente
});
```

```csharp
[HttpPost("analyze-consequences")]
public async Task<IActionResult> AnalyzeConsequences([FromForm] AnalyzeConsequencesRequest request)
```

### Opción 3: Aceptar string crudo

```csharp
[HttpPost("analyze-consequences")]
public async Task<IActionResult> AnalyzeConsequences([FromBody] string text)
{
    var request = new AnalyzeConsequencesRequest 
    { 
        Text = text,
        Context = "workplace",
        Country = "CO"
    };
    // ...
}
```

---

## ?? Resumen

El problema es que **JSON con saltos de línea no se está escapando correctamente** entre frontend y backend.

**Soluciones aplicadas:**
1. ? Normalizar saltos de línea en frontend
2. ? Agregar charset UTF-8
3. ? Remover validaciones estrictas del DTO
4. ? Validación manual en controller
5. ? Endpoint de test para debugging

**Próximo paso:**
- Reiniciar y probar `/api/AI/test-binding` primero
- Si funciona, entonces el problema es específico del modelo de IA
- Si no funciona, usar una de las opciones alternativas

---

**¿Cuál es el resultado al probar `/api/AI/test-binding`?** Eso nos dirá si el binding funciona o no.
