using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ContactService
    {
        private readonly IContactRepository _repo;

        public ContactService(IContactRepository repo)
        {
            _repo = repo;
        }

        public async Task<ContactResponse> CreateAsync(int usuarioId, CreateContactRequest req, CancellationToken ct = default)
        {
            if (await _repo.EmailExistsAsync(usuarioId, req.Email, null, ct))
                throw new InvalidOperationException("Ya existe un contacto con ese correo.");

            var entity = new Contact
            {
                UsuarioId = usuarioId,
                Name = req.Name.Trim(),
                Email = req.Email.Trim(),
                PhoneNumber = req.PhoneNumber?.Trim(),
                IsFavorite = req.IsFavorite,
                IsBlocked = req.IsBlocked
            };

            var saved = await _repo.AddAsync(entity, ct);
            return Map(saved);
        }

        public async Task<ContactResponse?> GetAsync(int id, int usuarioId, CancellationToken ct = default)
        {
            var c = await _repo.GetByIdAsync(id, usuarioId, ct);
            return c is null ? null : Map(c);
        }

        public async Task<PagedResult<ContactResponse>> SearchAsync(int usuarioId, string? q, int page, int pageSize, CancellationToken ct = default)
        {
            var (items, total) = await _repo.SearchAsync(usuarioId, q, page, pageSize, ct);
            return new PagedResult<ContactResponse>
            {
                Items = items.Select(Map).ToArray(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ContactResponse> UpdateAsync(int id, int usuarioId, UpdateContactRequest req, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, usuarioId, ct) 
                ?? throw new KeyNotFoundException("Contacto no encontrado.");

            if (await _repo.EmailExistsAsync(usuarioId, req.Email, id, ct))
                throw new InvalidOperationException("Ya existe otro contacto con ese correo.");

            entity.Name = req.Name.Trim();
            entity.Email = req.Email.Trim();
            entity.PhoneNumber = req.PhoneNumber?.Trim();
            entity.IsFavorite = req.IsFavorite;
            entity.IsBlocked = req.IsBlocked;

            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        public Task DeleteAsync(int id, int usuarioId, CancellationToken ct = default) 
            => _repo.DeleteAsync(id, usuarioId, ct);

        private static ContactResponse Map(Contact c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            IsFavorite = c.IsFavorite,
            IsBlocked = c.IsBlocked
        };
    }
}
