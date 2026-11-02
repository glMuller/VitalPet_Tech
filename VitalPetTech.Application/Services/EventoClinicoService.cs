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

public class EventoClinicoService : IEventoClinicoService
{
    private readonly IEventoClinicoRepository _repository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<EventoClinicoService> _logger;

    public EventoClinicoService(
        IEventoClinicoRepository repository,
        IPetRepository petRepository,
        ILogger<EventoClinicoService> logger)
    {
        _repository = repository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PagedResult<EventoClinicoResponse>> GetPagedAsync(EventoClinicoFilter filter)
    {
        PagingRules.Normalize(filter);
        var result = await _repository.GetPagedAsync(filter);
        return result.Map(e => e.ToResponse());
    }

    public async Task<EventoClinicoResponse> GetByIdAsync(int id) =>
        (await ObterOuFalharAsync(id)).ToResponse();

    public async Task<EventoClinicoResponse> CreateAsync(EventoClinicoRequest request)
    {
        var pet = await ObterPetOuFalharAsync(request.IdPet);

        var evento = request.ToEntity();
        evento.Pet = pet;

        await _repository.AddAsync(evento);
        _logger.LogInformation("Evento clínico {EventoId} criado para o pet {PetId}.", evento.Id, pet.Id);
        return evento.ToResponse();
    }

    public async Task UpdateAsync(int id, EventoClinicoRequest request)
    {
        var evento = await ObterOuFalharAsync(id);

        if (evento.IdPet != request.IdPet)
            evento.Pet = await ObterPetOuFalharAsync(request.IdPet);

        request.ApplyTo(evento);
        await _repository.UpdateAsync(evento);
        _logger.LogInformation("Evento clínico {EventoId} atualizado.", id);
    }

    public async Task DeleteAsync(int id)
    {
        var evento = await ObterOuFalharAsync(id);
        await _repository.DeleteAsync(evento);
        _logger.LogInformation("Evento clínico {EventoId} removido.", id);
    }

    private async Task<EventoClinico> ObterOuFalharAsync(int id)
    {
        var evento = await _repository.GetByIdAsync(id);
        if (evento is null)
        {
            _logger.LogWarning("Evento clínico com ID {EventoId} não foi encontrado.", id);
            throw new NotFoundException("Evento clínico não encontrado.");
        }
        return evento;
    }

    private async Task<Pet> ObterPetOuFalharAsync(int petId)
    {
        var pet = await _petRepository.GetByIdAsync(petId);
        if (pet is null)
            throw new BusinessRuleException($"O pet com ID {petId} não existe.");
        return pet;
    }
}