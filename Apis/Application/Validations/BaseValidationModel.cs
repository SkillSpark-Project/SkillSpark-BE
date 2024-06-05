using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations
{
    public abstract class BaseValidationModel<T> : IBaseValidationModel
{
    public async Task Validate(object validator, IBaseValidationModel modelObj)
    {
        var instance = (IValidator<T>)validator;
        var result = await instance.ValidateAsync((T)modelObj);
    
        if (!result.IsValid && result.Errors.Any())
        {
            throw new ValidationException(result.Errors);
        }
    }
}
}
