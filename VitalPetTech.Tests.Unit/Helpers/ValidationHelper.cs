using System.ComponentModel.DataAnnotations;

namespace VitalPetTech.Tests.Unit.Helpers;

public static class ValidationHelper
{
    // Roda as DataAnnotations manualmente, como o ASP.NET faz no model binding
    public static IList<ValidationResult> Validar(object model)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), resultados, validateAllProperties: true);
        return resultados;
    }
}