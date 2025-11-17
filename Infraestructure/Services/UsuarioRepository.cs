using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Services
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = "SELECT id, username, password_hash AS PasswordHash, email, rol, esta_activo AS EstaActivo, fecha_creacion AS FechaCreacion FROM usuarios WHERE username = @Username AND esta_activo = TRUE";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Username = username });
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = "SELECT id, username, password_hash AS PasswordHash, email, rol, esta_activo AS EstaActivo, fecha_creacion AS FechaCreacion FROM usuarios WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO usuarios (username, password_hash, email, rol, esta_activo, fecha_creacion)
                VALUES (@Username, @PasswordHash, @Email, @Rol, @EstaActivo, @FechaCreacion)
                RETURNING id, username, password_hash AS PasswordHash, email, rol, esta_activo AS EstaActivo, fecha_creacion AS FechaCreacion";
            
            return await connection.QuerySingleAsync<Usuario>(sql, usuario);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = "SELECT id, username, password_hash AS PasswordHash, email, rol, esta_activo AS EstaActivo, fecha_creacion AS FechaCreacion FROM usuarios ORDER BY fecha_creacion DESC";
            return await connection.QueryAsync<Usuario>(sql);
        }
    }
}
