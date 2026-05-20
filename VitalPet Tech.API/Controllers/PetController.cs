using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Entites;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PetController : ControllerBase
{
    private readonly ApplicationContext _context;

    public PetController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os pets cadastrados e as informações dos seus tutores.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll()
    {
        // Assumindo que o seu DbSet no ApplicationContext se chama Pets
        var pets = await _context.Pets
            .Include(p => p.Tutor) // Traz os dados do tutor dono do pet
            .ToListAsync();
            
        return Ok(pets);
    }

    /// <summary>
    /// Retorna um pet pelo ID e traz os dados do tutor dele.
    /// </summary>
    /// <param name="id">ID do pet</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pet>> GetById(int id)
    {
        var pet = await _context.Pets
            .Include(p => p.Tutor) 
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null)
            return NotFound("Pet não encontrado.");

        return Ok(pet);
    }
    
    /// <summary>
    /// Retorna todos os pets de uma espécie específica.
    /// </summary>
    /// <param name="especie">A espécie do pet (ex: Cachorro, Gato)</param>
    [HttpGet("especie/{especie}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByEspecie(string especie)
    {
        // O uso do .ToLower() garante que "Cachorro", "cachorro" ou "CACHORRO" retornem o mesmo resultado
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Where(p => p.EspeciePet.ToLower() == especie.ToLower())
            .ToListAsync();

        if (!pets.Any())
            return NotFound($"Nenhum pet da espécie '{especie}' foi encontrado.");

        return Ok(pets);
    }

    /// <summary>
    /// Retorna todos os pets associados a um tutor específico.
    /// </summary>
    /// <param name="tutorId">ID do tutor</param>
    [HttpGet("tutor/{tutorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByTutor(int tutorId)
    {
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Where(p => p.IdTutor == tutorId)
            .ToListAsync();

        if (!pets.Any())
            return NotFound($"Nenhum pet encontrado para o tutor com ID {tutorId}.");

        return Ok(pets);
    }

    /// <summary>
    /// Cria um novo pet.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pet>> Create(Pet pet)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Dica: O EF Core vai vincular automaticamente o Pet ao Tutor usando o IdTutor informado
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    /// <summary>
    /// Atualiza os dados de um pet existente.
    /// </summary>
    /// <param name="id">ID do pet a ser atualizado</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Pet pet)
    {
        if (id != pet.Id)
            return BadRequest("O ID da URL é diferente do ID do corpo da requisição.");

        _context.Entry(pet).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PetExists(id))
                return NotFound("Pet não encontrado para atualização.");
            else
                throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um pet pelo ID informado.
    /// </summary>
    /// <param name="id">ID do pet a ser removido</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        if (pet == null)
            return NotFound("Pet não encontrado.");

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Método auxiliar para checar existência no Update
    private bool PetExists(int id)
    {
        return _context.Pets.Any(e => e.Id == id);
    }
}