using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ThomasDeWulf.Tools.Core.Exceptions;
using ThomasDeWulf.Tools.Core.Responses;

namespace ThomasDeWulf.Tools.Core.Middleware
{
    public class ApplicationErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;

        public ApplicationErrorHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            this.next = next;
            this.env = env;
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
                await next(context);
            }
            catch (HttpStatusCodeException e) //An exception we made with a custom statusCode and/or message.
            {
               await HandleExceptionAsync(context, e);
            }
            catch (Exception e) //An other exception is thrown
            {
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
            var result = exception.Message;
            context.Response.ContentType = exception.ContentType;
            context.Response.StatusCode = (int) exception.StatusCode;
            
            return context.Response.WriteAsync(result);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            
            var response = new ErrorResponse
            {
                Message = "Er is iet fout gegaan. Probeer later opnieuw.",
                DetailedMessage = exception.Message
            };

            if (env.IsDevelopment())
            {
                response.StackTrace = exception.StackTrace;
            }

            await context.Response.WriteAsync(response.ToString());
        }
    }
}