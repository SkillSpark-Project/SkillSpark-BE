using Application.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.FilterAttibutes
{
    public class ModelValidatorAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var actionArgument in context.ActionArguments)
            {
                if (actionArgument.Value is IBaseValidationModel model)
                {
                    var modelType = actionArgument.Value.GetType();
                    var genericType = typeof(IValidator<>).MakeGenericType(modelType);
                    var validator = context.HttpContext.RequestServices.GetService(genericType);

                    if (validator != null)
                    {
                        // execute validator to validate model
                        await model.Validate(validator, model);
                    }
                }
            }
            await next();
        }
    }
}
