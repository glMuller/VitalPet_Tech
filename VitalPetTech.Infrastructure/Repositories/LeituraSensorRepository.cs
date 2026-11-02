using MongoDB.Bson;
using MongoDB.Driver;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;

namespace VitalPetTech.Infrastructure.Repositories;

public class LeituraSensorRepository : ILeituraSensorRepository
{
    private readonly IMongoCollection<LeituraSensor> _collection;

    public LeituraSensorRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<LeituraSensor>("leituras_sensor");
    }

    public Task AddAsync(LeituraSensor leitura) => _collection.InsertOneAsync(leitura);

    public async Task<LeituraSensor?> GetByIdAsync(string id)
    {
        // Um id que não é ObjectId válido não pode existir, e evita FormatException do driver
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _collection.Find(l => l.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<LeituraSensor>> GetPagedAsync(LeituraSensorFilter filter)
    {
        var fb = Builders<LeituraSensor>.Filter;
        var filtros = new List<FilterDefinition<LeituraSensor>>();

        if (filter.PetId.HasValue) filtros.Add(fb.Eq(l => l.IdPet, filter.PetId.Value));
        if (filter.Tipo.HasValue) filtros.Add(fb.Eq(l => l.Tipo, filter.Tipo.Value));
        if (filter.De.HasValue) filtros.Add(fb.Gte(l => l.Timestamp, filter.De.Value));
        if (filter.Ate.HasValue) filtros.Add(fb.Lte(l => l.Timestamp, filter.Ate.Value));

        var combinado = filtros.Count == 0 ? fb.Empty : fb.And(filtros);

        // Lista fixa de campos, e o padrão é a leitura mais recente primeiro
        var sb = Builders<LeituraSensor>.Sort;
        var ordenado = !string.IsNullOrWhiteSpace(filter.OrderBy);
        var desc = ordenado ? filter.Descending : true;

        var sort = (filter.OrderBy?.ToLower()) switch
        {
            "valor" => desc ? sb.Descending(l => l.Valor) : sb.Ascending(l => l.Valor),
            "tipo" => desc ? sb.Descending(l => l.Tipo) : sb.Ascending(l => l.Tipo),
            _ => desc ? sb.Descending(l => l.Timestamp) : sb.Ascending(l => l.Timestamp)
        };

        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

        var total = await _collection.CountDocumentsAsync(combinado);
        var itens = await _collection.Find(combinado)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<LeituraSensor>(itens, page, pageSize, (int)total);
    }
}