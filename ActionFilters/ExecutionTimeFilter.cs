using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_API.ActionFilters
{
    public class ExecutionTimeFilter : IActionFilter
    {
        DateTime startTime , endTime;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            startTime = DateTime.Now;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            endTime = DateTime.Now;
            Console.WriteLine($"Time : {startTime - endTime}");
        }
    }
}
