using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Services
{
    public class ContactRepositoryPostgres : IContactRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ContactRepositoryPostgres> _logger;

        public ContactRepositoryPostgres(IConfiguration configuration, ILogger<ContactRepositoryPostgres> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            _logger = logger;
        }

        public async Task<Contact> AddAsync(Contact contact, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO contactos (usuario_id, nombre, email, telefono, es_favorito, esta_bloqueado, fecha_creacion)
                VALUES (@UsuarioId, @Name, @Email, @PhoneNumber, @IsFavorite, @IsBlocked, CURRENT_TIMESTAMP)
                RETURNING id, usuario_id AS UsuarioId, nombre AS Name, email AS Email, telefono AS PhoneNumber, es_favorito AS IsFavorite, esta_bloqueado AS IsBlocked";
            
            return await connection.QuerySingleAsync<Contact>(sql, contact);
        }

        public async Task<Contact?> GetByIdAsync(int id, int usuarioId, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                SELECT id AS Id, usuario_id AS UsuarioId, nombre AS Name, email AS Email, 
                       telefono AS PhoneNumber, es_favorito AS IsFavorite, esta_bloqueado AS IsBlocked 
                FROM contactos 
                WHERE id = @Id AND usuario_id = @UsuarioId";
            
            return await connection.QueryFirstOrDefaultAsync<Contact>(sql, new { Id = id, UsuarioId = usuarioId });
        }

        public async Task UpdateAsync(Contact contact, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                UPDATE contactos 
                SET nombre = @Name, 
                    email = @Email, 
                    telefono = @PhoneNumber, 
                    es_favorito = @IsFavorite, 
                    esta_bloqueado = @IsBlocked
                WHERE id = @Id AND usuario_id = @UsuarioId";
            
            await connection.ExecuteAsync(sql, contact);
        }

        public async Task DeleteAsync(int id, int usuarioId, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = "DELETE FROM contactos WHERE id = @Id AND usuario_id = @UsuarioId";
            await connection.ExecuteAsync(sql, new { Id = id, UsuarioId = usuarioId });
        }

        public async Task<(IReadOnlyList<Contact> Items, int Total)> SearchAsync(
            int usuarioId, string? q, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            var whereClause = "WHERE usuario_id = @UsuarioId";
            if (!string.IsNullOrWhiteSpace(q))
            {
                whereClause += " AND (nombre ILIKE @Q OR email ILIKE @Q OR telefono ILIKE @Q)";
            }

            var countSql = $"SELECT COUNT(*) FROM contactos {whereClause}";
            var total = await connection.QuerySingleAsync<int>(countSql, new { UsuarioId = usuarioId, Q = $"%{q}%" });

            var sql = $@"
                SELECT id AS Id, usuario_id AS UsuarioId, nombre AS Name, email AS Email, 
                       telefono AS PhoneNumber, es_favorito AS IsFavorite, esta_bloqueado AS IsBlocked
                FROM contactos 
                {whereClause}
                ORDER BY es_favorito DESC, nombre ASC
                LIMIT @PageSize OFFSET @Offset";

            var items = await connection.QueryAsync<Contact>(sql, new
            {
                UsuarioId = usuarioId,
                Q = $"%{q}%",
                PageSize = pageSize,
                Offset = (page - 1) * pageSize
            });

            return (items.ToList(), total);
        }

        public async Task<IReadOnlyList<string>> GetEmailsByIdsAsync(int usuarioId, IEnumerable<int> ids, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var idList = ids.ToArray();
            
            if (idList.Length == 0) return Array.Empty<string>();

            const string sql = "SELECT email FROM contactos WHERE usuario_id = @UsuarioId AND id = ANY(@Ids) AND email IS NOT NULL";
            var emails = await connection.QueryAsync<string>(sql, new { UsuarioId = usuarioId, Ids = idList });
            
            return emails.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        public async Task<IReadOnlyList<string>> GetEmailsByNamesAsync(
            int usuarioId,
            IEnumerable<string> names,
            bool allowPartialMatch = true,
            CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var nameList = names.Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
            
            if (nameList.Length == 0) return Array.Empty<string>();

            var sql = allowPartialMatch
                ? "SELECT email FROM contactos WHERE usuario_id = @UsuarioId AND (nombre ILIKE ANY(@Names)) AND email IS NOT NULL"
                : "SELECT email FROM contactos WHERE usuario_id = @UsuarioId AND (LOWER(nombre) = ANY(@Names)) AND email IS NOT NULL";

            var searchNames = allowPartialMatch
                ? nameList.Select(n => $"%{n}%").ToArray()
                : nameList.Select(n => n.ToLowerInvariant()).ToArray();

            var emails = await connection.QueryAsync<string>(sql, new { UsuarioId = usuarioId, Names = searchNames });
            
            return emails.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        public async Task<bool> EmailExistsAsync(int usuarioId, string email, int? excludeId = null, CancellationToken ct = default)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            var sql = excludeId.HasValue
                ? "SELECT COUNT(*) FROM contactos WHERE usuario_id = @UsuarioId AND LOWER(email) = LOWER(@Email) AND id != @ExcludeId"
                : "SELECT COUNT(*) FROM contactos WHERE usuario_id = @UsuarioId AND LOWER(email) = LOWER(@Email)";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                UsuarioId = usuarioId,
                Email = email,
                ExcludeId = excludeId
            });

            return count > 0;
        }
    }
}
