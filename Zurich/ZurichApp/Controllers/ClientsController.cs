using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZurichApp.Api.Dtos.Clients;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientsController(IClientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{clientId:int}")]
        public async Task<ActionResult<ClientResponse>> GetById([FromRoute] int clientId)
        {
            var c = await _service.GetByIdAsync(clientId);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost("with-user")]
        public async Task<ActionResult<ClientResponse>> CreateWithUser([FromBody] ClientCreateWithUserRequest request)
        {
            var created = await _service.CreateClientWithUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { clientId = created.ClientId }, created);
        }

        [HttpPut("{clientId:int}")]
        public async Task<IActionResult> Update([FromRoute] int clientId, [FromBody] ClientUpdateRequest request)
        {
            await _service.UpdateAsync(clientId, request);
            return NoContent();
        }

        [HttpDelete("{clientId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int clientId)
        {
            await _service.DeleteAsync(clientId);
            return NoContent();
        }
    }
}
