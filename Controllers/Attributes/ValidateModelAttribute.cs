using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qydha.Models;

namespace Qydha.Controllers.Attributes;

public class ValidateModelAttribute : Attribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.SelectMany(e => e.Value!.Errors.Select(e => e.ErrorMessage)).ToList().Aggregate((acc, e) => $"{acc} ; {e} ");

            context.Result = new BadRequestObjectResult(new Error()
            {
                Code = ErrorCodes.InvalidBodyInput,
                Message = errors
            });
        }
    }
}
