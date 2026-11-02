using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;

namespace VitalPetTech.Infrastructure.Repositories;

internal static class QueryExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, PagedQuery paging)
    {
        var page = paging.Page < 1 ? 1 : paging.Page;
        var pageSize = paging.PageSize < 1 ? 10 : paging.PageSize;

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, page, pageSize, total);
    }
}