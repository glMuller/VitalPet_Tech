using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface IAlertaRepository : IRepository<Alerta>
{
    Task<PagedResult<Alerta>> GetPagedAsync(AlertaFilter filter);
}