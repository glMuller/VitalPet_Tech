using Microsoft.Extensions.Logging;
using VitalPetTech.Application.Common;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Application.Mappings;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Interfaces;

namespace VitalPetTech.Application.Services;

public class LeituraSensorService : ILeituraSensorService
{
    private readonly ILeituraSensorRepository _repository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<LeituraSensorService> _logger;

    public LeituraSensorService(
        ILeituraSensorRepository repository,
        IPetRepository petRepository,
        ILogger<LeituraSensorService> logger)
    {
        _repository = repository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PagedResult<LeituraSensorResponse>> GetPagedAsync(LeituraSensorFilter filter)
    {
        PagingRules.Normalize(filter);
        var result = await _repository.GetPagedAsync(filter);
        return result.Map(l => l.ToResponse());
    }

    public async Task<LeituraSensorResponse> GetByIdAsync(string id)
    {
        var leitura = await _repository.GetByIdAsync(id);
        if (leitura is null)
        {
            _logger.LogWarning("Leitura de sensor {LeituraId} não foi encontrada.", id);
            throw new NotFoundException("Leitura de sensor não encontrada.");
        }
        return leitura.ToResponse();
    }

    public async Task<LeituraSensorResponse> CreateAsync(LeituraSensorRequest request)
    {
        // O pet vive no Oracle e a leitura no Mongo, então não existe FK: a regra é validada aqui
        if (!await _petRepository.ExistsAsync(request.IdPet))
            throw new BusinessRuleException($"O pet com ID {request.IdPet} não existe.");

        var leitura = request.ToEntity();
        await _repository.AddAsync(leitura);

        _logger.LogInformation("Leitura {LeituraId} ({Tipo}) registrada para o pet {PetId}.",
            leitura.Id, leitura.Tipo, leitura.IdPet);
        return leitura.ToResponse();
    }
}