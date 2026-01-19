using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZurichApp.Api.Dtos.Quotes;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _service;

        public QuotesController(IQuoteService service)
        {
            _service = service;
        }

        // ✅ Admin: todas o por clientId (querystring)
        // - GET /api/quotes            -> todas
        // - GET /api/quotes?clientId=1 -> por cliente seleccionado
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuoteResponse>>> GetAllOrByClient([FromQuery] int? clientId)
        {
            if (clientId.HasValue)
            {
                var byClient = await _service.GetByClientIdAsync(clientId.Value);
                return Ok(byClient);
            }

            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        // ✅ Cliente: mis cotizaciones (clientId desde JWT)
        [Authorize(Roles = "Cliente")]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<QuoteResponse>>> GetMine()
        {
            var mine = await _service.GetMineAsync();
            return Ok(mine);
        }

        // Opcional: Admin crea cotización
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<QuoteResponse>> Create([FromBody] QuoteCreateRequest request)
        {
            var created = await _service.CreateAsync(request);
            return Ok(created);
        }
    }
}
