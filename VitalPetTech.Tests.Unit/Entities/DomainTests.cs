using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using Xunit;

namespace VitalPetTech.Tests.Unit.Entities;

public class AlertaTests
{
    [Fact]
    public void Construtor_NovoAlerta_NasceNaoResolvido()
    {
        // Arrange & Act
        var alerta = new Alerta();

        // Assert
        Assert.False(alerta.Resolvido);
    }
}

public class PagedResultTests
{
    [Fact]
    public void TotalPages_TotalNaoMultiploDoTamanho_ArredondaParaCima()
    {
        // Arrange
        var resultado = new PagedResult<int>(new List<int>(), page: 1, pageSize: 10, totalCount: 23);

        // Act
        var totalPaginas = resultado.TotalPages;

        // Assert
        Assert.Equal(3, totalPaginas);
    }

    [Fact]
    public void TotalPages_SemItens_RetornaZero()
    {
        // Arrange
        var resultado = new PagedResult<int>(new List<int>(), page: 1, pageSize: 10, totalCount: 0);

        // Act
        var totalPaginas = resultado.TotalPages;

        // Assert
        Assert.Equal(0, totalPaginas);
    }
}