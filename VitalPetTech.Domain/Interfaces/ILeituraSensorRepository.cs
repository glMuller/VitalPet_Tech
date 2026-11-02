using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface ILeituraSensorRepository
{
    Task AddAsync(LeituraSensor leitura);
    Task<LeituraSensor?> GetByIdAsync(string id);
    Task<PagedResult<LeituraSensor>> GetPagedAsync(LeituraSensorFilter filter);
}