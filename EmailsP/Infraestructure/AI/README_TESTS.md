# Tests para Local AI Server

## Descripción
Este archivo contiene tests unitarios para `local_ai_server.py` que **validan las rutas Flask y previenen cambios accidentales** en los endpoints.

## Qué se valida

### ✅ Rutas exactas
- `/health` debe existir exactamente en esa ruta (no `/Health`, `/HEALTH`, `/api/health`, etc.)
- `/generate` debe existir exactamente en esa ruta (no `/Generate`, `/api/generate`, etc.)

### ✅ Métodos HTTP
- `/health` **solo** acepta `GET` (rechaza POST, PUT, DELETE, PATCH)
- `/generate` **solo** acepta `POST` (rechaza GET, PUT, DELETE, PATCH)

### ✅ Formato de request/response
- `/generate` requiere JSON con campo `inputs`
- `/generate` rechaza inputs vacíos o solo espacios
- `/generate` acepta parámetros opcionales: `max_new_tokens`, `temperature`
- `/health` retorna JSON con `status` y `model`

### ✅ Manejo de errores
- Retorna 400 para requests inválidos
- Retorna 405 para métodos HTTP incorrectos
- Retorna 500 para errores del modelo
- Retorna 404 para rutas que no existen

## Por qué es importante

**Si alguien modifica las rutas en el código**, por ejemplo:
```python
# ❌ Si cambias esto:
@app.route("/generate", methods=["POST"])
# Por esto:
@app.route("/api/generate", methods=["POST"])
```

**Los tests FALLARÁN en el CI/CD** y el pipeline se detendrá, evitando que se despliegue código roto.

## Cómo ejecutar localmente

```bash
cd EmailsP/Infraestructure/AI
pip install -r requirements-test.txt
pytest test_local_ai_server.py -v
```

## Resultado esperado
```
19 passed in X.XXs
```

Todos los tests deben pasar para que el CI/CD continúe con el despliegue.
