using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public class AlertaRepository : BaseRepository<Alerta>, IAlertaRepository
{
    public AlertaRepository(ApplicationContext context) : base(context) { }

    public override Task<Alerta?> GetByIdAsync(int id) =>
        Context.Alertas.Include(a => a.Pet).FirstOrDefaultAsync(a => a.Id == id);

    public override Task<bool> ExistsAsync(int id) =>
        Context.Alertas.AnyAsync(a => a.Id == id);

    public async Task<PagedResult<Alerta>> GetPagedAsync(AlertaFilter filter)
    {
        var query = Context.Alertas.Include(a => a.Pet).AsNoTracking().AsQueryable();

        if (filter.PetId.HasValue)
            query = query.Where(a => a.IdPet == filter.PetId.Value);

        if (filter.Resolvido.HasValue)
            query = query.Where(a => a.Resolvido == filter.Resolvido.Value);

        if (filter.Severidade.HasValue)
            query = query.Where(a => a.Severidade == filter.Severidade.Value);

        if (filter.TipoAlerta.HasValue)
            query = query.Where(a => a.TipoAlerta == filter.TipoAlerta.Value);

        var ordenado = !string.IsNullOrWhiteSpace(filter.OrderBy);
        var desc = ordenado ? filter.Descending : true;

        query = (filter.OrderBy?.ToLower()) switch
        {
            "id" => desc ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
            "severidade" => desc ? query.OrderByDescending(a => a.Severidade) : query.OrderBy(a => a.Severidade),
            "tipo" => desc ? query.OrderByDescending(a => a.TipoAlerta) : query.OrderBy(a => a.TipoAlerta),
            _ => desc ? query.OrderByDescending(a => a.Timestamp) : query.OrderBy(a => a.Timestamp)
        };

        return await query.ToPagedResultAsync(filter);
    }
}