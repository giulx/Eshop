using Microsoft.AspNetCore.Mvc;
using ApiServer.Application.Interfaces;
using ApiServer.DTOs;

namespace ApiServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtenteController : ControllerBase
    {
        private readonly IUtenteService _utenteService;

        public UtenteController(IUtenteService utenteService)
        {
            _utenteService = utenteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utenti = await _utenteService.GetAllAsync();
            return Ok(utenti);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var utente = await _utenteService.GetByIdAsync(id);
            if (utente == null) return NotFound();
            return Ok(utente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UtenteUpdateDto dto)
        {
            var result = await _utenteService.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _utenteService.DeleteAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
