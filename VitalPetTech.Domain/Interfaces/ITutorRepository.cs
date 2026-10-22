using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface ITutorRepository : IRepository<Tutor>
{
    Task<PagedResult<Tutor>> GetPagedAsync(TutorFilter filter);
}