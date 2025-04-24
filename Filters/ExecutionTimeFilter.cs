using Microsoft.AspNetCore.Mvc.Filters;

namespace Test_API.ActionFilters
{
    public class ExecutionTimeFilter : ActionFilterAttribute
    {
        private DateTime _startTime;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _startTime = DateTime.Now;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var endTime = DateTime.Now;
            var duration = endTime - _startTime;
            Console.WriteLine($"Execution Time: {duration.TotalMilliseconds} ms");
        }
    }
}
