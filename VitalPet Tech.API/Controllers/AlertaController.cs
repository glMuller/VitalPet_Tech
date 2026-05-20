using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Entites;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertaController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AlertaController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os alertas registrados no sistema.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Alerta>>> GetAll()
    {
        var alertas = await _context.Alerta
            .Include(a => a.Pet)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
            
        return Ok(alertas);
    }

    /// <summary>
    /// Retorna um alerta específico pelo ID.
    /// </summary>
    /// <param name="id">ID do alerta</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Alerta>> GetById(int id)
    {
        var alerta = await _context.Alerta
            .Include(a => a.Pet)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alerta == null)
            return NotFound("Alerta não encontrado.");

        return Ok(alerta);
    }

    /// <summary>
    /// Retorna apenas os alertas que ainda não foram resolvidos pelo tutor.
    /// </summary>
    [HttpGet("pendentes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Alerta>>> GetPendentes()
    {
        var alertasPendentes = await _context.Alerta
            .Include(a => a.Pet)
            .Where(a => !a.Resolvido)
            .OrderByDescending(a => a.Timestamp) // Traz os mais recentes primeiro
            .ToListAsync();

        if (!alertasPendentes.Any())
            return NotFound("Não há alertas pendentes no momento.");

        return Ok(alertasPendentes);
    }

    /// <summary>
    /// Cria um novo registro de alerta (geralmente disparado pelo sensor IoT).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Alerta>> Create(Alerta alerta)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Garante que todo novo alerta nasça como não resolvido, por segurança
        alerta.Resolvido = false;

        _context.Alerta.Add(alerta);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = alerta.Id }, alerta);
    }

    /// <summary>
    /// Atualiza os dados de um alerta (ideal para marcar como Resolvido).
    /// </summary>
    /// <param name="id">ID do alerta a ser atualizado</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Alerta alerta)
    {
        if (id != alerta.Id)
            return BadRequest("O ID da URL é diferente do ID do corpo da requisição.");

        _context.Entry(alerta).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AlertaExists(id))
                return NotFound("Alerta não encontrado para atualização.");
            else
                throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um alerta pelo ID.
    /// </summary>
    /// <param name="id">ID do alerta a ser removido</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var alerta = await _context.Alerta.FindAsync(id);
        if (alerta == null)
            return NotFound("Alerta não encontrado.");

        _context.Alerta.Remove(alerta);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Método auxiliar para checar existência no Update
    private bool AlertaExists(int id)
    {
        return _context.Alerta.Any(e => e.Id == id);
    }
}