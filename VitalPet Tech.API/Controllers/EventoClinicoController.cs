using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Common;
using VitalPet_Tech.API.Hateoas;
using VitalPetTech.Application.Hateoas;
using Microsoft.AspNetCore.Authorization;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Authorize]
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
    [ProducesResponseType(typeof(PagedResponse<EventoClinicoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<EventoClinicoResponse>>> GetAll([FromQuery] EventoClinicoFilter filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(this.ToPagedResponse(result, WithLinks));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoClinicoResponse>> GetById(int id)
        => Ok(WithLinks(await _service.GetByIdAsync(id)));

    [HttpPost]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EventoClinicoResponse>> Create(EventoClinicoRequest request)
    {
        var created = WithLinks(await _service.CreateAsync(request));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private EventoClinicoResponse WithLinks(EventoClinicoResponse evento) => evento with
    {
        Links = new[]
        {
            this.LinkTo("self", "GET", nameof(GetById), values: new { id = evento.Id }),
            this.LinkTo("update", "PUT", nameof(Update), values: new { id = evento.Id }),
            this.LinkTo("delete", "DELETE", nameof(Delete), values: new { id = evento.Id }),
            this.LinkTo("pet", "GET", "GetById", "Pet", new { id = evento.IdPet })
        }
    };

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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}