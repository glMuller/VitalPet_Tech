using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Common;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventoClinicoController : ControllerBase
{
    private readonly IEventoClinicoService _service;

    public EventoClinicoController(IEventoClinicoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista eventos clínicos com paginação, ordenação (data, tipo, clinica, id)
    /// e filtros por pet e tipo. Por padrão, do mais recente para o mais antigo.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EventoClinicoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EventoClinicoResponse>>> GetAll([FromQuery] EventoClinicoFilter filter)
        => Ok(await _service.GetPagedAsync(filter));

    /// <summary>
    /// Retorna um evento clínico pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoClinicoResponse>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>
    /// Cria um novo evento clínico. O pet informado precisa existir.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EventoClinicoResponse>> Create(EventoClinicoRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um evento clínico existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, EventoClinicoRequest request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Remove um evento clínico pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}