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
public class PetController : ControllerBase
{
    private readonly IPetService _service;

    public PetController(IPetService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista pets com paginação, ordenação (id, nome, especie, peso, dataNascimento)
    /// e filtros por espécie e tutor.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PetResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PetResponse>>> GetAll([FromQuery] PetFilter filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(this.ToPagedResponse(result, WithLinks));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetResponse>> GetById(int id)
        => Ok(WithLinks(await _service.GetByIdAsync(id)));

    [HttpPost]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PetResponse>> Create(PetRequest request)
    {
        var created = WithLinks(await _service.CreateAsync(request));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private PetResponse WithLinks(PetResponse pet) => pet with
    {
        Links = new[]
        {
            this.LinkTo("self", "GET", nameof(GetById), values: new { id = pet.Id }),
            this.LinkTo("update", "PUT", nameof(Update), values: new { id = pet.Id }),
            this.LinkTo("delete", "DELETE", nameof(Delete), values: new { id = pet.Id }),
            this.LinkTo("tutor", "GET", "GetById", "Tutor", new { id = pet.IdTutor }),
            this.LinkTo("eventos-clinicos", "GET", "GetAll", "EventoClinico", new { petId = pet.Id }),
            this.LinkTo("alertas", "GET", "GetAll", "Alerta", new { petId = pet.Id })
        }
    };

    /// <summary>
    /// Atualiza os dados de um pet existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, PetRequest request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Remove um pet pelo ID.
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