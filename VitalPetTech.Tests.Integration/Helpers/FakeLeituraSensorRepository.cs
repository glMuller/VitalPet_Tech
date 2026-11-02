using System.Collections.Concurrent;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;

namespace VitalPetTech.Tests.Integration.Helpers;

// Substitui o MongoDB nos testes de integração
public class FakeLeituraSensorRepository : ILeituraSensorRepository
{
    private readonly ConcurrentDictionary<string, LeituraSensor> _dados = new();

    public Task AddAsync(LeituraSensor leitura)
    {
        leitura.Id = Guid.NewGuid().ToString("N")[..24];
        _dados[leitura.Id] = leitura;
        return Task.CompletedTask;
    }

    public Task<LeituraSensor?> GetByIdAsync(string id) =>
        Task.FromResult(_dados.TryGetValue(id, out var l) ? l : null);

    public Task<PagedResult<LeituraSensor>> GetPagedAsync(LeituraSensorFilter filter)
    {
        var query = _dados.Values.AsEnumerable();
        if (filter.PetId.HasValue) query = query.Where(l => l.IdPet == filter.PetId.Value);
        if (filter.Tipo.HasValue) query = query.Where(l => l.Tipo == filter.Tipo.Value);

        var ordenados = query.OrderByDescending(l => l.Timestamp).ToList();
        var itens = ordenados.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        return Task.FromResult(new PagedResult<LeituraSensor>(itens, filter.Page, filter.PageSize, ordenados.Count));
    }
}