using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using VitalPetTech.Application.Hateoas;
using VitalPetTech.Domain.Common;

namespace VitalPet_Tech.API.Hateoas;

// Lista paginada com links de navegação
public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<Link> Links);

public static class HateoasExtensions
{
    // Gera a URL de uma action (do mesmo controller ou de outro) e monta o Link
    public static Link LinkTo(
        this ControllerBase controller,
        string rel,
        string method,
        string action,
        string? controllerName = null,
        object? values = null)
    {
        var href = controller.Url.Action(action, controllerName, values, controller.Request.Scheme)
                   ?? string.Empty;
        return new Link(href, rel, method);
    }

    public static PagedResponse<T> ToPagedResponse<T>(
        this ControllerBase controller, PagedResult<T> result, Func<T, T> addLinks)
    {
        var items = result.Items.Select(addLinks).ToList();
        return new PagedResponse<T>(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            BuildPageLinks(controller.Request, result));
    }

    private static List<Link> BuildPageLinks<T>(HttpRequest request, PagedResult<T> result)
    {
        var baseUri = $"{request.Scheme}://{request.Host}{request.Path}";

        // Mantém filtros e ordenação da requisição original, trocando só a página
        string PageUrl(int page)
        {
            var query = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var (key, value) in request.Query)
                query[key] = value.ToString();

            query["page"] = page.ToString();
            query["pageSize"] = result.PageSize.ToString();
            return QueryHelpers.AddQueryString(baseUri, query);
        }

        var links = new List<Link> { new(PageUrl(result.Page), "self", "GET") };

        if (result.TotalPages > 0)
        {
            links.Add(new(PageUrl(1), "first", "GET"));
            links.Add(new(PageUrl(result.TotalPages), "last", "GET"));

            if (result.Page > 1)
                links.Add(new(PageUrl(Math.Min(result.Page - 1, result.TotalPages)), "prev", "GET"));

            if (result.Page < result.TotalPages)
                links.Add(new(PageUrl(result.Page + 1), "next", "GET"));
        }

        links.Add(new(baseUri, "create", "POST"));
        return links;
    }
}