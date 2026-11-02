using Microsoft.Extensions.Logging;
using VitalPetTech.Application.Common;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Application.Mappings;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;

namespace VitalPetTech.Application.Services;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _repository;
    private readonly ILogger<TutorService> _logger;

    public TutorService(ITutorRepository repository, ILogger<TutorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<TutorResponse>> GetPagedAsync(TutorFilter filter)
    {
        PagingRules.Normalize(filter);
        var result = await _repository.GetPagedAsync(filter);
        return result.Map(t => t.ToResponse());
    }

    public async Task<TutorResponse> GetByIdAsync(int id) =>
        (await ObterOuFalharAsync(id)).ToResponse();

    public async Task<TutorResponse> CreateAsync(TutorRequest request)
    {
        var tutor = request.ToEntity();
        await _repository.AddAsync(tutor);
        _logger.LogInformation("Tutor {TutorId} criado.", tutor.Id);
        return tutor.ToResponse();
    }

    public async Task UpdateAsync(int id, TutorRequest request)
    {
        var tutor = await ObterOuFalharAsync(id);
        request.ApplyTo(tutor);
        await _repository.UpdateAsync(tutor);
        _logger.LogInformation("Tutor {TutorId} atualizado.", id);
    }

    public async Task DeleteAsync(int id)
    {
        var tutor = await ObterOuFalharAsync(id);
        await _repository.DeleteAsync(tutor);
        _logger.LogInformation("Tutor {TutorId} removido.", id);
    }

    private async Task<Tutor> ObterOuFalharAsync(int id)
    {
        var tutor = await _repository.GetByIdAsync(id);
        if (tutor is null)
        {
            _logger.LogWarning("Tutor com ID {TutorId} não foi encontrado.", id);
            throw new NotFoundException("Tutor não encontrado.");
        }
        return tutor;
    }
}