namespace AuthorsWebApi.Middelwares
{
    public static class HttpMiddelwareResponseExtensions
    {
        public static IApplicationBuilder UseHttpResponse(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseMiddleware<HttpMiddelwareResponse>();
        }
    }

    public class HttpMiddelwareResponse
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpMiddelwareResponse> _logger;

        public HttpMiddelwareResponse(RequestDelegate next, ILogger<HttpMiddelwareResponse> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalResponse = context.Response.Body;
                context.Response.Body = ms;

                await _next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalResponse);
                context.Response.Body = originalResponse;

                _logger.LogInformation(response);
            }
        }
    }
}