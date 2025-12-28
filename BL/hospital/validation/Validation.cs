using System.ComponentModel.DataAnnotations;
using Domain.hospital;

namespace BL.hospital.validation;

public class Validation<T> : IValidation<T> where T : BaseEntity
{
    private readonly IServiceProvider? _serviceProvider;

    public Validation(IServiceProvider? serviceProvider = null)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<ValidationResult> Validate(T baseEntity)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(baseEntity, serviceProvider: _serviceProvider, items: null);
        Validator.TryValidateObject(baseEntity, context, results, validateAllProperties: true);

        return results;
    }
}