using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Interfaces;

public interface ITutorService
{
    Task<PagedResult<TutorResponse>> GetPagedAsync(TutorFilter filter);
    Task<TutorResponse> GetByIdAsync(int id);
    Task<TutorResponse> CreateAsync(TutorRequest request);
    Task UpdateAsync(int id, TutorRequest request);
    Task DeleteAsync(int id);
}