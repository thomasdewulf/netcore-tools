using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThomasDeWulf.Tools.Core.Helpers;

namespace ThomasDeWulf.Tools.Core.Filters
{
    /// <summary>
    /// Custom attribute that stops the request pipeline if the ModelState isn't valid. Returns a 400 Bad Request.
    /// </summary>
    public class ModelStateIsValidAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorList = ModelStateFormatter.FormatErrors(context.ModelState);
                context.Result = new BadRequestObjectResult(errorList);
            }
            base.OnActionExecuting(context);
        }
    }
}
