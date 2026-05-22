using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/animais")]
[Produces("application/json")]
public class AnimaisController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedSpecies =
    {
        "DOG",
        "CAT"
    };

    public AnimaisController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAll()
    {
        var animais = await _context.Animais
            .AsNoTracking()
            .ToListAsync();

        return Ok(animais);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Animal), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Animal>> GetById(int id)
    {
        var animal = await _context.Animais
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (animal is null)
        {
            return NotFound();
        }

        return Ok(animal);
    }

    [HttpGet("responsavel/{responsavelId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetByResponsavelId(int responsavelId)
    {
        var responsavelExists = await _context.Responsaveis
            .CountAsync(t => t.Id == responsavelId) > 0;

        if (!responsavelExists)
        {
            return NotFound("Responsavel not found.");
        }

        var animais = await _context.Animais
            .AsNoTracking()
            .Where(p => p.ResponsavelId == responsavelId)
            .ToListAsync();

        return Ok(animais);
    }

    [HttpGet("species/{species}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetBySpecies(string species)
    {
        var normalizedSpecies = species.ToUpper();

        var animais = await _context.Animais
            .AsNoTracking()
            .Where(p => p.Species == normalizedSpecies)
            .ToListAsync();

        return Ok(animais);
    }

    [HttpGet("breed/{breed}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetByBreed(string breed)
    {
        var normalizedBreed = breed.ToLower();

        var animais = await _context.Animais
            .AsNoTracking()
            .Where(p => p.Breed.ToLower() == normalizedBreed)
            .ToListAsync();

        return Ok(animais);
    }

    [HttpGet("rga/{rga}")]
    [ProducesResponseType(typeof(Animal), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Animal>> GetByRga(string rga)
    {
        var animal = await _context.Animais
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Rga == rga);

        if (animal is null)
        {
            return NotFound();
        }

        return Ok(animal);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Animal), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Animal>> Create(Animal animal)
    {
        var responsavelExists = await _context.Responsaveis
            .CountAsync(t => t.Id == animal.ResponsavelId) > 0;

        if (!responsavelExists)
        {
            return BadRequest("ResponsavelId does not exist.");
        }

        var validationError = ValidateAnimal(animal);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var rgaAlreadyExists = !string.IsNullOrWhiteSpace(animal.Rga) &&
            await _context.Animais.CountAsync(p => p.Rga == animal.Rga) > 0;

        if (rgaAlreadyExists)
        {
            return BadRequest("RGA already registered.");
        }

        animal.Id = 0;
        animal.Species = animal.Species.ToUpper();
        animal.Sex = animal.Sex.ToUpper();
        animal.CreatedAt = DateTime.UtcNow;
        animal.IsActive = true;

        _context.Animais.Add(animal);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Animal animalAtualizado)
    {
        var animal = await _context.Animais
            .FirstOrDefaultAsync(p => p.Id == id);

        if (animal is null)
        {
            return NotFound();
        }

        var responsavelExists = await _context.Responsaveis
            .CountAsync(t => t.Id == animalAtualizado.ResponsavelId) > 0;

        if (!responsavelExists)
        {
            return BadRequest("ResponsavelId does not exist.");
        }

        var validationError = ValidateAnimal(animalAtualizado);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var rgaAlreadyExists = !string.IsNullOrWhiteSpace(animalAtualizado.Rga) &&
            await _context.Animais.CountAsync(p => p.Rga == animalAtualizado.Rga && p.Id != id) > 0;

        if (rgaAlreadyExists)
        {
            return BadRequest("RGA already registered by another animal.");
        }

        animal.ResponsavelId = animalAtualizado.ResponsavelId;
        animal.Name = animalAtualizado.Name;
        animal.Nickname = animalAtualizado.Nickname;
        animal.Species = animalAtualizado.Species.ToUpper();
        animal.Breed = animalAtualizado.Breed;
        animal.BirthDate = animalAtualizado.BirthDate;
        animal.Weight = animalAtualizado.Weight;
        animal.Sex = animalAtualizado.Sex.ToUpper();
        animal.Rga = animalAtualizado.Rga;
        animal.IsActive = animalAtualizado.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _context.Animais
            .FirstOrDefaultAsync(p => p.Id == id);

        if (animal is null)
        {
            return NotFound();
        }

        _context.Animais.Remove(animal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidateAnimal(Animal animal)
    {
        if (animal.ResponsavelId <= 0 ||
            string.IsNullOrWhiteSpace(animal.Name) ||
            string.IsNullOrWhiteSpace(animal.Species) ||
            string.IsNullOrWhiteSpace(animal.Breed) ||
            string.IsNullOrWhiteSpace(animal.Sex) ||
            animal.Weight <= 0)
        {
            return "ResponsavelId, name, species, breed, sex and valid weight are required.";
        }

        var normalizedSpecies = animal.Species.ToUpper();

        if (!AllowedSpecies.Contains(normalizedSpecies))
        {
            return "Species must be DOG or CAT in this MVP version.";
        }

        return null;
    }
}