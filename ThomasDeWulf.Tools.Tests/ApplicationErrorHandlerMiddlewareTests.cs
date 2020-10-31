using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ThomasDeWulf.Tools.Core.Exceptions;
using ThomasDeWulf.Tools.Core.Middleware;
using ThomasDeWulf.Tools.Core.Responses;
using Xunit;

namespace ThomasDeWulf.Tools.Tests
{
    public class ApplicationErrorHandlerMiddlewareTests
    {
        [Fact]
        public async Task MiddleWare_Should_Return_Custom_Response_And_StatusCode_When_Exception_Is_Thrown()
        {
            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            var logger = new Mock<ILogger<ApplicationErrorHandlerMiddleware>>();
            
            var middleware = new ApplicationErrorHandlerMiddleware((innerContext) => throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Resource not found"), hostingEnvironment.Object, logger.Object);
            
            var context = new DefaultHttpContext();
            
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(context.Response.Body);

            var streamText = reader.ReadToEnd();

            var objResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            
            objResponse
                .Should()
                .BeEquivalentTo(new ErrorResponse
                {
                    Message = "Resource not found",
                });

            context.Response.StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Middleware_Should_Return_InternalServerError_When_Generic_Exception_Is_Thrown()
        {
            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            var logger = new Mock<ILogger<ApplicationErrorHandlerMiddleware>>();
            
            var middleware = new ApplicationErrorHandlerMiddleware((innerContext) => throw new Exception( "Big exception"), hostingEnvironment.Object, logger.Object);
            
            var context = new DefaultHttpContext();
            
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(context.Response.Body);

            var streamText = reader.ReadToEnd();

            var objResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);

            objResponse
                .Should()
                .BeOfType<ErrorResponse>();

            context.Response.StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError);
        }


        [Fact]
        public async Task StackTrace_Should_Not_Be_Null_When_Environment_Is_Development()
        {
            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            hostingEnvironment.Setup(x => x.EnvironmentName).Returns("Development");

            var logger = new Mock<ILogger<ApplicationErrorHandlerMiddleware>>();

            var middleware = new ApplicationErrorHandlerMiddleware(
                (innerContext) => throw new Exception("Big exception"), hostingEnvironment.Object, logger.Object);

            var context = new DefaultHttpContext();

            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(context.Response.Body);

            var streamText = reader.ReadToEnd();

            var objResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            
            objResponse.StackTrace
                .Should()
                .NotBeNull();

        }

        [Fact]
        public async Task Middleware_Should_Not_Execute_When_No_Exception_Is_Thrown()
        {
            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            var logger = new Mock<ILogger<ApplicationErrorHandlerMiddleware>>();
            
            var middleware = new ApplicationErrorHandlerMiddleware((innerHttpContext) =>
            {
                innerHttpContext.Response.WriteAsync("No exception!");
                return Task.CompletedTask;
            }, hostingEnvironment.Object, logger.Object);
            
            var context = new DefaultHttpContext();
            
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(context.Response.Body);

            var streamText = reader.ReadToEnd();

            streamText
                .Should()
                .BeEquivalentTo("No exception!");
        }

        [Fact]
        public async Task ContentType_Is_Correctly_Set_In_Custom_Error_Response()
        {
            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            var logger = new Mock<ILogger<ApplicationErrorHandlerMiddleware>>();
            
            var customException = new HttpStatusCodeException(HttpStatusCode.Forbidden)
            {
                ContentType = "application/xml"
            };
            
            var middleware = new ApplicationErrorHandlerMiddleware((innerContext) => throw customException, hostingEnvironment.Object, logger.Object);
            
            var context = new DefaultHttpContext();
            
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);


            context.Response.ContentType
                .Should()
                .BeEquivalentTo("application/xml");
        }
    }
}