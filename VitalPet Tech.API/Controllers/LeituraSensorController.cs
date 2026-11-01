using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalPet_Tech.API.Hateoas;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Common;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class LeituraSensorController : ControllerBase
{
    private readonly ILeituraSensorService _service;

    public LeituraSensorController(ILeituraSensorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista leituras dos sensores (MongoDB) com paginação, ordenação (timestamp, valor, tipo)
    /// e filtros por pet, tipo e período. Por padrão, da mais recente para a mais antiga.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<LeituraSensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<LeituraSensorResponse>>> GetAll([FromQuery] LeituraSensorFilter filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(this.ToPagedResponse(result, WithLinks));
    }

    /// <summary>
    /// Retorna uma leitura pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeituraSensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraSensorResponse>> GetById(string id)
        => Ok(WithLinks(await _service.GetByIdAsync(id)));

    /// <summary>
    /// Registra uma leitura de sensor IoT. O pet informado precisa existir.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeituraSensorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LeituraSensorResponse>> Create(LeituraSensorRequest request)
    {
        var created = WithLinks(await _service.CreateAsync(request));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private LeituraSensorResponse WithLinks(LeituraSensorResponse leitura) => leitura with
    {
        Links = new[]
        {
            this.LinkTo("self", "GET", nameof(GetById), values: new { id = leitura.Id }),
            this.LinkTo("pet", "GET", "GetById", "Pet", new { id = leitura.IdPet })
        }
    };
}