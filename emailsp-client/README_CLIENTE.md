# 📧 EmailsP - Cliente Vue 3

Cliente web profesional para consumir la API EmailsP, con diseño inspirado en el modo incógnito de Chrome.

## ✨ Características

- 🎨 **Diseño Modo Incógnito**: Estética oscura profesional similar a Chrome
- 🔐 **Autenticación JWT**: Login seguro con tokens
- 📨 **Envío de Emails**: Con y sin adjuntos (hasta 25MB)
- 👥 **Gestión de Contactos**: CRUD completo + favoritos + bloqueos
- 🤖 **IA - Reformulador**: Convierte texto informal a formal
- 📊 **IA - Analizador**: Evalúa riesgos legales, emocionales y efectividad

## 🛠️ Tecnologías

- **Vue 3** - Framework progresivo
- **Vue Router** - Navegación SPA
- **Pinia** - Gestión de estado
- **Tailwind CSS** - Estilos utility-first
- **Axios** - Cliente HTTP
- **Vite** - Build tool ultrarrápido

## 📋 Requisitos Previos

- Node.js 18+ 
- npm o yarn
- API EmailsP corriendo en `https://localhost:7268`

## 🚀 Instalación

```bash
# Navegar al directorio del proyecto
cd emailsp-client

# Las dependencias ya están instaladas
# Si necesitas reinstalar:
npm install
```

## ▶️ Ejecutar Proyecto

```bash
# Modo desarrollo
npm run dev

# El proyecto estará en: http://localhost:5173
```

## 🔑 Credenciales por Defecto

```
Usuario: admin
Contraseña: admin123
```

## 📁 Estructura del Proyecto

```
src/
├── components/          # Componentes reutilizables
│   ├── AppSidebar.vue  # Sidebar de navegación
│   └── MainLayout.vue  # Layout principal
├── views/              # Vistas/Páginas
│   ├── LoginView.vue
│   ├── SendEmailView.vue
│   ├── ContactsView.vue
│   ├── RefactorTextView.vue
│   └── AnalyzeMessageView.vue
├── services/           # Servicios API
│   ├── api.js          # Cliente Axios
│   ├── authService.js  # Autenticación
│   ├── emailService.js # Emails
│   ├── contactService.js # Contactos
│   └── aiService.js    # IA
├── stores/             # Pinia stores
│   └── auth.js         # Estado de autenticación
└── router/             # Configuración de rutas
    └── index.ts
```

## 🎨 Paleta de Colores (Modo Incógnito)

```css
--incognito-darker: #121212
--incognito-dark: #202124
--incognito-medium: #292A2D
--incognito-light: #3C4043
--incognito-text: #E8EAED
--incognito-subtext: #9AA0A6
--incognito-accent: #8AB4F8
```

## 📱 Funcionalidades

### 1️⃣ Enviar Correo
- Múltiples destinatarios
- Adjuntos (hasta 25MB total)
- Soporte HTML en el cuerpo
- Validación de formulario

### 2️⃣ Contactos
- Crear, editar, eliminar contactos
- Marcar favoritos ⭐
- Bloquear contactos 🚫
- Búsqueda y paginación

### 3️⃣ Formalizar Texto
- IA convierte texto informal → formal
- Ejemplos precargados
- Copiar resultado
- Usar directamente en email

### 4️⃣ Analizar Mensaje
- Riesgo legal
- Impacto emocional
- Efectividad del mensaje
- Riesgo de represalias
- Recomendaciones accionables
- Contexto colombiano/latinoamericano

## 🔧 Configuración de la API

Por defecto, el cliente se conecta a:
```
https://localhost:7268
```

Para cambiar la URL, edita `src/services/api.js`:
```javascript
const API_BASE_URL = 'https://tu-api-url-aqui'
```

## 🚨 Certificados SSL

Si la API usa HTTPS con certificado autofirmado, el navegador puede bloquearlo.

**Solución temporal (desarrollo):**
1. Abre `https://localhost:7268` en el navegador
2. Acepta el certificado no seguro
3. Vuelve al cliente

## 📦 Build para Producción

```bash
npm run build

# Los archivos estarán en dist/
```

## 🤝 Mejores Prácticas

- ✅ Componentes reutilizables
- ✅ Composables para lógica compartida
- ✅ Servicios separados por dominio
- ✅ Manejo global de errores
- ✅ Interceptores de Axios
- ✅ Rutas protegidas con guards
- ✅ Estado centralizado con Pinia
- ✅ Diseño responsive

## 🐛 Troubleshooting

### Error de CORS
Si ves errores de CORS, asegúrate de que la API tenga configurado:
```csharp
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### Token expirado
Los tokens expiran en 60 minutos. El sistema te redirigirá automáticamente al login.

### Error al enviar emails
Verifica:
- Configuración SMTP en la API
- Gmail App Password correcto
- Tamaño de adjuntos < 25MB

## 📚 Recursos

- [Vue 3 Docs](https://vuejs.org/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Pinia](https://pinia.vuejs.org/)
- [Axios](https://axios-http.com/)

## 👨‍💻 Autor

Proyecto desarrollado siguiendo las mejores prácticas de Vue 3 y Clean Architecture.
