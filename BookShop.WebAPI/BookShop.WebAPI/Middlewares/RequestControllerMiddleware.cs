using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace BookShop.WebAPI.Middlewares
{
    public class RequestControllerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestControllerMiddleware> _logger;

        public RequestControllerMiddleware(RequestDelegate next, ILogger<RequestControllerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Включаем буферизацию тела запроса, чтобы его можно было прочитать несколько раз
                context.Request.EnableBuffering();

                // Чтение заголовков
                var headers = context.Request.Headers;

                /*
                 * Используем StreamReader для чтения тела запроса.
                 * Устанавливаем leaveOpen: true, чтобы сохранить поток открытым для следующего middleware.
                 * После чтения устанавливаем позицию потока на 0.
                */
                //Чтение тела запроса
                string requestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Сброс потока для следующего middleware
                }



                // Логируем информацию о запросе
                var clientIp = context.Connection.RemoteIpAddress?.ToString();
                var requestMethod = context.Request.Method;
                var requestUrl = context.Request.Path;
                //var userAgent = headers["User-Agent"].ToString();
                var queryString = context.Request.QueryString.ToString();

                _logger.LogInformation($"Incoming request from {clientIp}: {requestMethod} {requestUrl} {queryString} ");


                var watch = Stopwatch.StartNew();



                //Обработка запроса
                await _next(context);

                watch.Stop();

                var responseStatusCode = context.Response.StatusCode;

                _logger.LogInformation($"Response status code: {responseStatusCode} - Elapsed time: {watch.ElapsedMilliseconds} ms");
                //await _next(context); // Передача управления следующему компоненту в конвейере
            }
            catch ( Exception ex ) 
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred. in request middleware");
            }
        }
    }
}
