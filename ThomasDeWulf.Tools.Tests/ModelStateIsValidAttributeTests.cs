using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThomasDeWulf.Tools.Core.Filters;
using Xunit;

namespace ThomasDeWulf.Tools.Tests
{
    public class ModelStateIsValidAttributeTests
    {
        [Fact]
        public void Attribute_Should_Return_BadRequest_When_Model_Has_Errors()
        {
            var modelState = new ModelStateDictionary();
            
            modelState.AddModelError("", "This is the error message");
            
            var httpContext = new DefaultHttpContext();
            
            var actionContext = new ActionExecutingContext(
                    new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelState ),
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object>(),
                    new Mock<ControllerBase>().Object
                );
            
            var attribute = new ModelStateIsValidAttribute();
            
            attribute.OnActionExecuting(actionContext);

            actionContext.Result
                .Should()
                .BeOfType<BadRequestObjectResult>();
        }
    }
}