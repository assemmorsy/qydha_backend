using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qydha.Models;

namespace Qydha.Controllers.Attributes;

public class ExceptionHandlerAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {

        var Exception = context.Exception;
        Console.WriteLine(Exception);
        while (Exception.InnerException != null)
        {
            Exception = Exception.InnerException;
        }

        // TODO :: DO SOME LOGGING HERE 
        context.Result = new BadRequestObjectResult(new Error()
        {
            Message = Exception.Message,
            Code = ErrorCodes.UnknownError
        });
    }
}
