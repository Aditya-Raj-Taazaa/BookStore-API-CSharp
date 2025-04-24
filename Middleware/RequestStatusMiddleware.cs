using Test_API.Services;

namespace Test_API.Middleware
{
    public class RequestStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppInfoService _appInfoService;
        private readonly RequestAuditService _requestAuditService;

        public RequestStatusMiddleware(RequestDelegate next, AppInfoService appInfoService, RequestAuditService requestAuditService)
        {
            _next = next;
            _appInfoService = appInfoService;
            _requestAuditService = requestAuditService;
        }

        static string StatusColor(string method)
        {
            return method switch
            {
                "GET" => "Green",
                "PUT" => "Blue",
                "DELETE" => "Red",
                _ => "Yellow",
            };
        }

        static int LogWriteHelper(HttpContext http)
        {
            var request = http.Request;
            string value = request.Path.Value ?? string.Empty;
            int id = 0, multiplier = 1;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                char c = value[i];
                if (char.IsDigit(c))
                {
                    id += (c - '0') * multiplier;
                    multiplier *= 10;
                }
                else
                    break;
            }
            return id;
        }

        public async Task Invoke(HttpContext context)
        {
            DateTime startTime = DateTime.Now;

            context.Response.Headers["X-App-Name"] = _appInfoService.GetAppName();
            context.Response.Headers["X-App-Version"] = _appInfoService.GetVersion();

            Console.WriteLine("🔀 Middleware Begins");

            await _next(context);

            var response = context.Response;
            var request = context.Request;

            var host = request.Headers.FirstOrDefault(h => h.Key == "Host").Value;

            int id = LogWriteHelper(context);

            if (request.Method == "GET" && id != 0)
                _requestAuditService.LogWrite("Request", id);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Time Stamp : {startTime} \n");

            string colorCode = StatusColor(request.Method);

            if (Enum.TryParse(colorCode, true, out ConsoleColor parsedColor))
                Console.ForegroundColor = parsedColor;
            else
                Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine($"Method: {request.Method}");
            Console.WriteLine($"Endpoint: {host}{request.Path.Value}");
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Content Type: {response.ContentType}");
            Console.WriteLine($"Request Cookies: {string.Join(", ", request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
            Console.WriteLine($"Request Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
            Console.ResetColor();
        }
    }
}