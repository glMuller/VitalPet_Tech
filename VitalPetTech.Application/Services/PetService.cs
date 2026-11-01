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

public class PetService : IPetService
{
    private readonly IPetRepository _repository;
    private readonly ITutorRepository _tutorRepository;
    private readonly ILogger<PetService> _logger;

    public PetService(IPetRepository repository, ITutorRepository tutorRepository, ILogger<PetService> logger)
    {
        _repository = repository;
        _tutorRepository = tutorRepository;
        _logger = logger;
    }

    public async Task<PagedResult<PetResponse>> GetPagedAsync(PetFilter filter)
    {
        PagingRules.Normalize(filter);
        var result = await _repository.GetPagedAsync(filter);
        return result.Map(p => p.ToResponse());
    }

    public async Task<PetResponse> GetByIdAsync(int id) =>
        (await ObterOuFalharAsync(id)).ToResponse();

    public async Task<PetResponse> CreateAsync(PetRequest request)
    {
        var tutor = await ObterTutorOuFalharAsync(request.IdTutor);

        var pet = request.ToEntity();
        pet.Tutor = tutor;

        await _repository.AddAsync(pet);
        _logger.LogInformation("Pet {PetId} criado para o tutor {TutorId}.", pet.Id, tutor.Id);
        return pet.ToResponse();
    }

    public async Task UpdateAsync(int id, PetRequest request)
    {
        var pet = await ObterOuFalharAsync(id);

        if (pet.IdTutor != request.IdTutor)
            pet.Tutor = await ObterTutorOuFalharAsync(request.IdTutor);

        request.ApplyTo(pet);
        await _repository.UpdateAsync(pet);
        _logger.LogInformation("Pet {PetId} atualizado.", id);
    }

    public async Task DeleteAsync(int id)
    {
        var pet = await ObterOuFalharAsync(id);
        await _repository.DeleteAsync(pet);
        _logger.LogInformation("Pet {PetId} removido.", id);
    }

    private async Task<Pet> ObterOuFalharAsync(int id)
    {
        var pet = await _repository.GetByIdAsync(id);
        if (pet is null)
        {
            _logger.LogWarning("Pet com ID {PetId} não foi encontrado.", id);
            throw new NotFoundException("Pet não encontrado.");
        }
        return pet;
    }

    private async Task<Tutor> ObterTutorOuFalharAsync(int tutorId)
    {
        var tutor = await _tutorRepository.GetByIdAsync(tutorId);
        if (tutor is null)
            throw new BusinessRuleException($"O tutor com ID {tutorId} não existe.");
        return tutor;
    }
}