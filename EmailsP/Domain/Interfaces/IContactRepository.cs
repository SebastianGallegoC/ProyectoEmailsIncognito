using Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> AddAsync(Contact contact, CancellationToken ct = default);
        Task<Contact?> GetByIdAsync(int id, int usuarioId, CancellationToken ct = default);
        Task UpdateAsync(Contact contact, CancellationToken ct = default);
        Task DeleteAsync(int id, int usuarioId, CancellationToken ct = default);

        Task<(IReadOnlyList<Contact> Items, int Total)> SearchAsync(
            int usuarioId, string? q, int page, int pageSize, CancellationToken ct = default);

        Task<IReadOnlyList<string>> GetEmailsByIdsAsync(int usuarioId, IEnumerable<int> ids, CancellationToken ct = default);

        Task<IReadOnlyList<string>> GetEmailsByNamesAsync(
            int usuarioId,
            IEnumerable<string> names,
            bool allowPartialMatch = true,
            CancellationToken ct = default);

        Task<bool> EmailExistsAsync(int usuarioId, string email, int? excludeId = null, CancellationToken ct = default);
    }
}
