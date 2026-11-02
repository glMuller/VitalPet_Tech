using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface IEventoClinicoRepository : IRepository<EventoClinico>
{
    Task<PagedResult<EventoClinico>> GetPagedAsync(EventoClinicoFilter filter);
}