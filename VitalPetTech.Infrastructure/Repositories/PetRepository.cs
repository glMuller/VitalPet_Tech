using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public class PetRepository : BaseRepository<Pet>, IPetRepository
{
    public PetRepository(ApplicationContext context) : base(context) { }

    public override Task<Pet?> GetByIdAsync(int id) =>
        Context.Pets.Include(p => p.Tutor).FirstOrDefaultAsync(p => p.Id == id);

    public override Task<bool> ExistsAsync(int id) =>
        Context.Pets.AnyAsync(p => p.Id == id);

    public async Task<PagedResult<Pet>> GetPagedAsync(PetFilter filter)
    {
        var query = Context.Pets.Include(p => p.Tutor).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Especie))
        {
            var especie = filter.Especie.ToLower();
            query = query.Where(p => p.EspeciePet.ToLower() == especie);
        }

        if (filter.TutorId.HasValue)
            query = query.Where(p => p.IdTutor == filter.TutorId.Value);

        query = (filter.OrderBy?.ToLower()) switch
        {
            "id" => filter.Descending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
            "especie" => filter.Descending ? query.OrderByDescending(p => p.EspeciePet) : query.OrderBy(p => p.EspeciePet),
            "peso" => filter.Descending ? query.OrderByDescending(p => p.PesoPet) : query.OrderBy(p => p.PesoPet),
            "datanascimento" => filter.Descending ? query.OrderByDescending(p => p.DataNascimento) : query.OrderBy(p => p.DataNascimento),
            _ => filter.Descending ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome)
        };

        return await query.ToPagedResultAsync(filter);
    }
}