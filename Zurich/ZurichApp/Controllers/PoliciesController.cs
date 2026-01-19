using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZurichApp.Api.Dtos.Policies;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _service;

        public PoliciesController(IPolicyService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PolicyResponse>>> GetAllOrByClient([FromQuery] int? clientId)
        {
            if (clientId.HasValue)
            {
                var byClient = await _service.GetByClientIdAsync(clientId.Value);
                return Ok(byClient);
            }

            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        [Authorize(Roles = "Cliente")]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<PolicyResponse>>> GetMine()
        {
            var mine = await _service.GetMineAsync();
            return Ok(mine);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<PolicyResponse>> Create([FromBody] PolicyCreateRequest request)
        {
            var created = await _service.CreateAsync(request);
            return Ok(created);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("{policyId:int}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int policyId)
        {
            await _service.CancelAsync(policyId);
            return NoContent();
        }

    }
}
