using Microsoft.AspNetCore.Mvc;
using ApiServer.Application.Interfaces;
using ApiServer.DTOs;

namespace ApiServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdineController : ControllerBase
    {
        private readonly IOrdineService _ordineService;

        public OrdineController(IOrdineService ordineService)
        {
            _ordineService = ordineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ordini = await _ordineService.GetAllAsync();
            return Ok(ordini);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ordine = await _ordineService.GetByIdAsync(id);
            if (ordine == null) return NotFound();
            return Ok(ordine);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdineCreateDto dto)
        {
            var result = await _ordineService.CreateAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdineUpdateDto dto)
        {
            var result = await _ordineService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ordineService.DeleteAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
