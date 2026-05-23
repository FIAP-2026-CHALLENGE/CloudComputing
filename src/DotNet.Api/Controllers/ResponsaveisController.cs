using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/responsaveis")]
[Produces("application/json")]
public class ResponsaveisController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResponsaveisController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Responsavel>), 200)]
    public async Task<ActionResult<IEnumerable<Responsavel>>> GetAll()
    {
        var responsaveis = await _context.Responsaveis
            .AsNoTracking()
            .ToListAsync();

        return Ok(responsaveis);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetById(int id)
    {
        var responsavel = await _context.Responsaveis
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        return Ok(responsavel);
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetByCpf(string cpf)
    {
        var responsavel = await _context.Responsaveis
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Cpf == cpf);

        if (responsavel is null)
        {
            return NotFound();
        }

        return Ok(responsavel);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Responsavel), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Responsavel>> Create(Responsavel responsavel)
    {
        if (string.IsNullOrWhiteSpace(responsavel.Name) ||
            string.IsNullOrWhiteSpace(responsavel.Email) ||
            string.IsNullOrWhiteSpace(responsavel.Phone) ||
            string.IsNullOrWhiteSpace(responsavel.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        var cpfAlreadyExists = await _context.Responsaveis
            .CountAsync(t => t.Cpf == responsavel.Cpf) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered.");
        }

        responsavel.Id = 0;
        responsavel.CreatedAt = DateTime.UtcNow;
        responsavel.IsActive = true;

        _context.Responsaveis.Add(responsavel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = responsavel.Id }, responsavel);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Responsavel responsavelAtualizado)
    {
        var responsavel = await _context.Responsaveis
            .FirstOrDefaultAsync(t => t.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(responsavelAtualizado.Name) ||
            string.IsNullOrWhiteSpace(responsavelAtualizado.Email) ||
            string.IsNullOrWhiteSpace(responsavelAtualizado.Phone) ||
            string.IsNullOrWhiteSpace(responsavelAtualizado.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        var cpfAlreadyExists = await _context.Responsaveis
            .CountAsync(t => t.Cpf == responsavelAtualizado.Cpf && t.Id != id) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered by another responsável.");
        }

        responsavel.Name = responsavelAtualizado.Name;
        responsavel.Email = responsavelAtualizado.Email;
        responsavel.Phone = responsavelAtualizado.Phone;
        responsavel.Cpf = responsavelAtualizado.Cpf;
        responsavel.IsActive = responsavelAtualizado.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var responsavel = await _context.Responsaveis
            .FirstOrDefaultAsync(t => t.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        _context.Responsaveis.Remove(responsavel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}