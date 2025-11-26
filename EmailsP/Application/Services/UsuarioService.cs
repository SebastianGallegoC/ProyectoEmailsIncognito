using Application.DTOs;
using BCrypt.Net;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Validar si el usuario ya existe
            var existingUserByUsername = await _repository.GetByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            var existingUserByEmail = await _repository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("El email ya está en uso");

            // Hash de la contraseña con BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Crear entidad
            var usuario = new Usuario
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Email,
                Rol = "User", // Por defecto User
                EstaActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            // Guardar en base de datos
            var created = await _repository.CreateAsync(usuario);

            return new RegisterResponse
            {
                Id = created.Id,
                Username = created.Username,
                Email = created.Email,
                Rol = created.Rol
            };
        }
    }
}