using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Interfaces;

public interface IAlertaService
{
    Task<PagedResult<AlertaResponse>> GetPagedAsync(AlertaFilter filter);
    Task<AlertaResponse> GetByIdAsync(int id);
    Task<AlertaResponse> CreateAsync(AlertaRequest request);
    Task UpdateAsync(int id, AlertaUpdateRequest request);
    Task DeleteAsync(int id);
}