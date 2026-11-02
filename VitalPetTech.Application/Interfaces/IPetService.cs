using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Interfaces;

public interface IPetService
{
    Task<PagedResult<PetResponse>> GetPagedAsync(PetFilter filter);
    Task<PetResponse> GetByIdAsync(int id);
    Task<PetResponse> CreateAsync(PetRequest request);
    Task UpdateAsync(int id, PetRequest request);
    Task DeleteAsync(int id);
}