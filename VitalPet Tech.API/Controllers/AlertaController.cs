using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Common;
using VitalPet_Tech.API.Hateoas;
using VitalPetTech.Application.Hateoas;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertaController : ControllerBase
{
    private readonly IAlertaService _service;

    public AlertaController(IAlertaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista alertas com paginação, ordenação (timestamp, severidade, tipo, id) e filtros
    /// por pet, resolvido, severidade e tipo. Use ?resolvido=false para ver os pendentes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AlertaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AlertaResponse>>> GetAll([FromQuery] AlertaFilter filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(this.ToPagedResponse(result, WithLinks));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaResponse>> GetById(int id)
        => Ok(WithLinks(await _service.GetByIdAsync(id)));

    [HttpPost]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AlertaResponse>> Create(AlertaRequest request)
    {
        var created = WithLinks(await _service.CreateAsync(request));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private AlertaResponse WithLinks(AlertaResponse alerta)
    {
        var links = new List<Link>
        {
            this.LinkTo("self", "GET", nameof(GetById), values: new { id = alerta.Id }),
            this.LinkTo("update", "PUT", nameof(Update), values: new { id = alerta.Id }),
            this.LinkTo("delete", "DELETE", nameof(Delete), values: new { id = alerta.Id }),
            this.LinkTo("pet", "GET", "GetById", "Pet", new { id = alerta.IdPet })
        };

        if (!alerta.Resolvido)
            links.Add(this.LinkTo("resolver", "PUT", nameof(Update), values: new { id = alerta.Id }));

        return alerta with { Links = links };
    }

    /// <summary>
    /// Atualiza um alerta (ideal para marcar como resolvido).
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, AlertaUpdateRequest request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Remove um alerta pelo ID.
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