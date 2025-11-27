using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailsP.Controllers
{
    [ApiController]
    [Route("api/[controllerxx]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UsuarioService _usuarioService;

        public AuthController(AuthService authService, UsuarioService usuarioService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Login de usuario
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateAsync(request);
            if (result == null)
                return Unauthorized(new { error = "Credenciales inválidas" });

            return Ok(result);
        }

        /// <summary>
        /// Registro de nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
                return BadRequest(new { error = "El username debe tener al menos 3 caracteres" });

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
                return BadRequest(new { error = "La contraseña debe tener al menos 4 caracteres" });

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                return BadRequest(new { error = "El email no es válido" });

            try
            {
                var registeredUser = await _usuarioService.RegisterAsync(request);

                // Después del registro, autenticar para obtener el token
                var loginRequest = new LoginRequest { Username = request.Username, Password = request.Password };
                var loginResponse = await _authService.AuthenticateAsync(loginRequest);

                if (loginResponse == null)
                {
                    // Esto no debería ocurrir si el registro fue exitoso, pero es una salvaguarda
                    return BadRequest(new { error = "Error al iniciar sesión después del registro." });
                }

                // El test espera una propiedad 'token', que está en LoginResponse
                return CreatedAtAction(nameof(Login), new { username = registeredUser.Username }, loginResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
