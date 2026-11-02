using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Interfaces;

public interface ILeituraSensorService
{
    Task<PagedResult<LeituraSensorResponse>> GetPagedAsync(LeituraSensorFilter filter);
    Task<LeituraSensorResponse> GetByIdAsync(string id);
    Task<LeituraSensorResponse> CreateAsync(LeituraSensorRequest request);
}