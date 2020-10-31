using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ThomasDeWulf.Tools.Core.Helpers;
using Xunit;

namespace ThomasDeWulf.Tools.Tests
{
    public class ModelStateFormatterTests
    {
        [Fact]
        public void Formatter_Should_Format_Single_Error_Correctly()
        {
            var modelState = new ModelStateDictionary();
            
            modelState.AddModelError("", "This is an error message");
            
            var list = new List<string>();
            list.Add("This is an error message");

            var formattedMessage = ModelStateFormatter.FormatErrors(modelState);

            formattedMessage
                .Should()
                .BeEquivalentTo(list);
        }

        [Fact]
        public void Formatter_Should_Format_Multiple_Errors_Correctly()
        {
            var modelState = new ModelStateDictionary();
            var list = new List<string>();

            var baseMessage = "This is error";

            for (int i = 0; i < 10; i++)
            {
                var fullMessage = $"{baseMessage} {i}";
                list.Add(fullMessage);
                modelState.AddModelError("",fullMessage);
            }

            var formattedMessage = ModelStateFormatter.FormatErrors(modelState);

            formattedMessage
                .Should()
                .BeEquivalentTo(list);
        }
    }
}