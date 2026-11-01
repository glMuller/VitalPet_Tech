using VitalPetTech.Application.Common;
using VitalPetTech.Domain.Common;
using Xunit;

namespace VitalPetTech.Tests.Unit.Common;

public class PagingRulesTests
{
    [Fact]
    public void Normalize_PageSizeAcimaDoMaximo_LimitaNoMaximo()
    {
        // Arrange
        var filtro = new PetFilter { PageSize = 500 };

        // Act
        PagingRules.Normalize(filtro);

        // Assert
        Assert.Equal(PagingRules.MaxPageSize, filtro.PageSize);
    }

    [Fact]
    public void Normalize_PaginaZero_AjustaParaUm()
    {
        // Arrange
        var filtro = new PetFilter { Page = 0 };

        // Act
        PagingRules.Normalize(filtro);

        // Assert
        Assert.Equal(1, filtro.Page);
    }

    [Fact]
    public void Normalize_PageSizeZero_UsaTamanhoPadrao()
    {
        // Arrange
        var filtro = new PetFilter { PageSize = 0 };

        // Act
        PagingRules.Normalize(filtro);

        // Assert
        Assert.Equal(PagingRules.DefaultPageSize, filtro.PageSize);
    }
}