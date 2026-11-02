using VitalPetTech.Domain.Common;

namespace VitalPetTech.Application.Common;

public static class PagingRules
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    public static void Normalize(PagedQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = DefaultPageSize;
        if (query.PageSize > MaxPageSize) query.PageSize = MaxPageSize;
    }
}