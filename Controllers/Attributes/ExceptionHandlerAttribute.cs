using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qydha.Models;

namespace Qydha.Controllers.Attributes;

public class ExceptionHandlerAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerAttribute> _logger;
    public ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger)
    {
        _logger = logger;
    }
    public override void OnException(ExceptionContext context)
    {

        var Exception = context.Exception;
        _logger.LogError(Exception, "Error caught by Exception Filter ");
        while (Exception.InnerException != null)
        {
            Exception = Exception.InnerException;
        }

        context.Result = new BadRequestObjectResult(new Error()
        {
            Message = Exception.Message,
            Code = ErrorCodes.UnknownError
        });
    }
}
