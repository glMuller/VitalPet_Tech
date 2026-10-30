using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Common;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TutorController : ControllerBase
{
    private readonly ITutorService _service;

    public TutorController(ITutorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista tutores com paginação, ordenação (id, nome) e filtro por nome.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TutorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TutorResponse>>> GetAll([FromQuery] TutorFilter filter)
        => Ok(await _service.GetPagedAsync(filter));

    /// <summary>
    /// Retorna um tutor pelo ID, com o resumo dos seus pets.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TutorResponse>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>
    /// Cria um novo tutor.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TutorResponse>> Create(TutorRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza os dados de um tutor existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, TutorRequest request)
    {
        await _service.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Remove um tutor (e, em cascata, seus pets, eventos e alertas).
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