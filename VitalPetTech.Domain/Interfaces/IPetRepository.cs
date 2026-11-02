using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface IPetRepository : IRepository<Pet>
{
    Task<PagedResult<Pet>> GetPagedAsync(PetFilter filter);
}