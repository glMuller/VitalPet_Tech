using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Entites;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventoClinicoController : ControllerBase
{
    private readonly ApplicationContext _context;

    public EventoClinicoController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os eventos clínicos cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventoClinico>>> GetAll()
    {
        var eventos = await _context.EventoClinico
            .Include(e => e.Pet) // Traz os dados do Pet vinculado ao evento
            .ToListAsync();
            
        return Ok(eventos);
    }

    /// <summary>
    /// Retorna um evento clínico específico pelo ID.
    /// </summary>
    /// <param name="id">ID do evento clínico</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoClinico>> GetById(int id)
    {
        var evento = await _context.EventoClinico
            .Include(e => e.Pet)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
            return NotFound("Evento clínico não encontrado.");

        return Ok(evento);
    }

    /// <summary>
    /// Retorna o histórico de eventos clínicos de um pet específico.
    /// </summary>
    /// <param name="petId">ID do pet</param>
    [HttpGet("pet/{petId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EventoClinico>>> GetByPet(int petId)
    { 
        var eventos = await _context.EventoClinico
            .Include(e => e.Pet)
            .Where(e => e.IdPet == petId)
            .OrderByDescending(e => e.DataEvento) // Retorna os mais recentes primeiro
            .ToListAsync();

        if (!eventos.Any())
            return NotFound($"Nenhum evento clínico encontrado para o pet com ID {petId}.");

        return Ok(eventos);
    }

    /// <summary>
    /// Cria um novo registro de evento clínico.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventoClinico>> Create(EventoClinico evento)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.EventoClinico.Add(evento);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = evento.Id }, evento);
    }

    /// <summary>
    /// Atualiza os dados de um evento clínico existente.
    /// </summary>
    /// <param name="id">ID do evento a ser atualizado</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, EventoClinico evento)
    {
        if (id != evento.Id)
            return BadRequest("O ID da URL é diferente do ID do corpo da requisição.");

        _context.Entry(evento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventoExists(id))
                return NotFound("Evento clínico não encontrado para atualização.");
            else
                throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um evento clínico pelo ID.
    /// </summary>
    /// <param name="id">ID do evento a ser removido</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _context.EventoClinico.FindAsync(id);
        if (evento == null)
            return NotFound("Evento clínico não encontrado.");

        _context.EventoClinico.Remove(evento);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Método auxiliar para checar existência no Update
    private bool EventoExists(int id)
    {
        return _context.EventoClinico.Any(e => e.Id == id);
    }
}