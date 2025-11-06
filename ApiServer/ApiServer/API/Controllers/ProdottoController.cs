using Microsoft.AspNetCore.Mvc;
using ApiServer.Application.Interfaces;
using ApiServer.DTOs;

namespace ApiServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdottoController : ControllerBase
    {
        private readonly IProdottoService _prodottoService;

        public ProdottoController(IProdottoService prodottoService)
        {
            _prodottoService = prodottoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prodotti = await _prodottoService.GetAllAsync();
            return Ok(prodotti);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prodotto = await _prodottoService.GetByIdAsync(id);
            if (prodotto == null) return NotFound();
            return Ok(prodotto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProdottoCreateDto dto)
        {
            var result = await _prodottoService.CreateAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProdottoUpdateDto dto)
        {
            var result = await _prodottoService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _prodottoService.DeleteAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
