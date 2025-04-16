using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Test_API.ExceptionFilters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred.");

            var result = new ObjectResult(new
            {
                Message = "Something went wrong. Please try again later.",
                Details = context.Exception.Message // Optional: remove in production
            })
            {
                StatusCode = 500
            };

            context.Result = result;
            Console.WriteLine("Exception Filter Called : ");
            context.ExceptionHandled = true;
        }
    }
}
