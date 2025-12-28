using System.ComponentModel.DataAnnotations;
using Domain.hospital;

namespace BL.hospital.validation;

public interface IValidation<T> where T : BaseEntity
{
    IEnumerable<ValidationResult> Validate(T entity);
}