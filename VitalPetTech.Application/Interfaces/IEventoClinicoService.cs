using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Interfaces;

public interface IEventoClinicoService
{
    Task<PagedResult<EventoClinicoResponse>> GetPagedAsync(EventoClinicoFilter filter);
    Task<EventoClinicoResponse> GetByIdAsync(int id);
    Task<EventoClinicoResponse> CreateAsync(EventoClinicoRequest request);
    Task UpdateAsync(int id, EventoClinicoRequest request);
    Task DeleteAsync(int id);
}