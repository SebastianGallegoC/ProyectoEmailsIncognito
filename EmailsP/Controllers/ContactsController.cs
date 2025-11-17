using Application.DTOs;
using Application.Services;
using EmailsP.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailsP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly ContactService _svc;

        public ContactsController(ContactService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateContactRequest req)
        {
            var usuarioId = User.GetUsuarioId();
            var created = await _svc.CreateAsync(usuarioId, req);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var usuarioId = User.GetUsuarioId();
            var item = await _svc.GetAsync(id, usuarioId);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ContactResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var usuarioId = User.GetUsuarioId();
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 || pageSize > 200 ? 20 : pageSize;

            var result = await _svc.SearchAsync(usuarioId, q, page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateContactRequest req)
        {
            var usuarioId = User.GetUsuarioId();
            var updated = await _svc.UpdateAsync(id, usuarioId, req);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var usuarioId = User.GetUsuarioId();
            await _svc.DeleteAsync(id, usuarioId);
            return NoContent();
        }
    }
}
