using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Entites;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TutorController : ControllerBase
{
    private readonly ApplicationContext _context;

    public TutorController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os tutores e seus respectivos pets.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Tutor>>> GetAll()
    {
        var tutores = await _context.Tutor
            .Include(t => t.Pets) // Traz os pets de todos os tutores
            .ToListAsync();
            
        return Ok(tutores);
    }

    /// <summary>
    /// Retorna um tutor pelo ID e traz a lista de pets dele.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tutor>> GetById(int id)
    {
        var tutor = await _context.Tutor
            .Include(t => t.Pets) // Traz os pets específicos desse tutor
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        return Ok(tutor);
    }

    /// <summary>
    /// Cria um novo tutor.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Tutor>> Create(Tutor tutor)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Tutor.Add(tutor);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, tutor);
    }

    /// <summary>
    /// Atualiza os dados de um tutor existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Tutor tutor)
    {
        if (id != tutor.Id)
            return BadRequest("O ID da URL é diferente do ID do corpo da requisição.");

        _context.Entry(tutor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TutorExists(id))
                return NotFound("Tutor não encontrado para atualização.");
            else
                throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um tutor pelo ID informado.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var tutor = await _context.Tutor.FindAsync(id);
        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        _context.Tutor.Remove(tutor);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool TutorExists(int id)
    {
        return _context.Tutor.Any(e => e.Id == id);
    }
}