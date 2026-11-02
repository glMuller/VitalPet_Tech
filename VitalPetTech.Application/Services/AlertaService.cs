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

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository _repository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<AlertaService> _logger;

    public AlertaService(
        IAlertaRepository repository,
        IPetRepository petRepository,
        ILogger<AlertaService> logger)
    {
        _repository = repository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PagedResult<AlertaResponse>> GetPagedAsync(AlertaFilter filter)
    {
        PagingRules.Normalize(filter);
        var result = await _repository.GetPagedAsync(filter);
        return result.Map(a => a.ToResponse());
    }

    public async Task<AlertaResponse> GetByIdAsync(int id) =>
        (await ObterOuFalharAsync(id)).ToResponse();

    public async Task<AlertaResponse> CreateAsync(AlertaRequest request)
    {
        var pet = await ObterPetOuFalharAsync(request.IdPet);

        var alerta = request.ToEntity(); // nasce com Resolvido = false
        alerta.Pet = pet;

        await _repository.AddAsync(alerta);
        _logger.LogInformation("Alerta {AlertaId} ({Severidade}) criado para o pet {PetId}.",
            alerta.Id, alerta.Severidade, pet.Id);
        return alerta.ToResponse();
    }

    public async Task UpdateAsync(int id, AlertaUpdateRequest request)
    {
        var alerta = await ObterOuFalharAsync(id);

        if (alerta.IdPet != request.IdPet)
            alerta.Pet = await ObterPetOuFalharAsync(request.IdPet);

        request.ApplyTo(alerta);
        alerta.Resolvido = request.Resolvido;

        await _repository.UpdateAsync(alerta);
        _logger.LogInformation("Alerta {AlertaId} atualizado (Resolvido: {Resolvido}).", id, alerta.Resolvido);
    }

    public async Task DeleteAsync(int id)
    {
        var alerta = await ObterOuFalharAsync(id);
        await _repository.DeleteAsync(alerta);
        _logger.LogInformation("Alerta {AlertaId} removido.", id);
    }

    private async Task<Alerta> ObterOuFalharAsync(int id)
    {
        var alerta = await _repository.GetByIdAsync(id);
        if (alerta is null)
        {
            _logger.LogWarning("Alerta com ID {AlertaId} não foi encontrado.", id);
            throw new NotFoundException("Alerta não encontrado.");
        }
        return alerta;
    }

    private async Task<Pet> ObterPetOuFalharAsync(int petId)
    {
        var pet = await _petRepository.GetByIdAsync(petId);
        if (pet is null)
            throw new BusinessRuleException($"O pet com ID {petId} não existe.");
        return pet;
    }
}