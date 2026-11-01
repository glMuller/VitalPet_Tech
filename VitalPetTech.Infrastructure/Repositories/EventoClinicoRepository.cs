using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public class EventoClinicoRepository : BaseRepository<EventoClinico>, IEventoClinicoRepository
{
    public EventoClinicoRepository(ApplicationContext context) : base(context) { }

    public override Task<EventoClinico?> GetByIdAsync(int id) =>
        Context.EventosClinicos.Include(e => e.Pet).FirstOrDefaultAsync(e => e.Id == id);

    public override Task<bool> ExistsAsync(int id) =>
        Context.EventosClinicos.AnyAsync(e => e.Id == id);

    public async Task<PagedResult<EventoClinico>> GetPagedAsync(EventoClinicoFilter filter)
    {
        var query = Context.EventosClinicos.Include(e => e.Pet).AsNoTracking().AsQueryable();

        if (filter.PetId.HasValue)
            query = query.Where(e => e.IdPet == filter.PetId.Value);

        if (filter.Tipo.HasValue)
            query = query.Where(e => e.Tipo == filter.Tipo.Value);

        // Sem orderBy informado, vale o padrão: mais recente primeiro.
        // Com orderBy informado, "Descending" é respeitado como veio.
        var ordenado = !string.IsNullOrWhiteSpace(filter.OrderBy);
        var desc = ordenado ? filter.Descending : true;

        query = (filter.OrderBy?.ToLower()) switch
        {
            "id" => desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
            "tipo" => desc ? query.OrderByDescending(e => e.Tipo) : query.OrderBy(e => e.Tipo),
            "clinica" => desc ? query.OrderByDescending(e => e.Clinica) : query.OrderBy(e => e.Clinica),
            _ => desc ? query.OrderByDescending(e => e.DataEvento) : query.OrderBy(e => e.DataEvento)
        };

        return await query.ToPagedResultAsync(filter);
    }
}