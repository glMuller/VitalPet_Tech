using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public class TutorRepository : BaseRepository<Tutor>, ITutorRepository
{
    public TutorRepository(ApplicationContext context) : base(context) { }

    public override Task<Tutor?> GetByIdAsync(int id) =>
        Context.Tutores.Include(t => t.Pets).FirstOrDefaultAsync(t => t.Id == id);

    public override Task<bool> ExistsAsync(int id) =>
        Context.Tutores.AnyAsync(t => t.Id == id);

    public async Task<PagedResult<Tutor>> GetPagedAsync(TutorFilter filter)
    {
        var query = Context.Tutores.Include(t => t.Pets).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Nome))
        {
            var nome = filter.Nome.ToLower();
            query = query.Where(t => t.NomeTutor.ToLower().Contains(nome));
        }

        query = (filter.OrderBy?.ToLower()) switch
        {
            "id" => filter.Descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
            _ => filter.Descending ? query.OrderByDescending(t => t.NomeTutor) : query.OrderBy(t => t.NomeTutor)
        };

        return await query.ToPagedResultAsync(filter);
    }
}