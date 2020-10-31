using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThomasDeWulf.Tools.Core.Exceptions;
using ThomasDeWulf.Tools.Core.Responses;

namespace ThomasDeWulf.Tools.Core.Middleware
{
    public class ApplicationErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApplicationErrorHandlerMiddleware> _logger;

        public ApplicationErrorHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ApplicationErrorHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// The request passes here. We catch all exceptions, divided into: our own exceptions and all other exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpStatusCodeException e) //An exception we made with a custom statusCode and/or message.
            {
                _logger.LogWarning(e.ToString());
               await HandleExceptionAsync(context, e);
            }
            catch (Exception e) //An other exception is thrown
            {
                _logger.LogError(e.ToString());
                await HandleExceptionAsync(context, e);
            }
        }

        /// <summary>
        /// Handles the exception if it is an exception we created (e.g. entity not found).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
        {
            var result = new ErrorResponse
            {
                Message = exception.Message
            };
            context.Response.ContentType = exception.ContentType;
            context.Response.StatusCode = (int) exception.StatusCode;
            
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            
            var response = new ErrorResponse
            {
                Message = "Er is iet fout gegaan. Probeer later opnieuw.",
                DetailedMessage = exception.Message
            };

            if (_env.IsDevelopment())
            {
                response.StackTrace = exception.StackTrace;
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
