using Formula.SimpleCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

#pragma warning disable 1591
namespace Formula.SimpleAPI
{
    public class ValidationFailureResponse
    {
        protected Status<string> results = null;

        public ValidationFailureResponse()
        {
            results = new Status<string>();
        }

        public ValidationFailureResponse AddFailure(string message, string key)
        {
            results.RecordFailure(message, key);
            return this;
        }

        public Status<string> ParseFromModelState(ActionContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                var errors = context.ModelState.Keys.Select(e => "" + e + "");

                foreach (var modelStatekey in context.ModelState.Keys)
                {
                    var entry = context.ModelState[modelStatekey];
                    if (entry.ValidationState == ModelValidationState.Invalid)
                    {
                        foreach (var err in entry.Errors)
                        {
                            AddFailure(err.ErrorMessage, modelStatekey);
                        }
                    }
                }
            }

            return results;
        }

        public IActionResult GetResponseContext()
        {
            var context = new ObjectResult(results);

            context.StatusCode = StatusCodes.Status422UnprocessableEntity;

            var message = "One or more validation errors occurred.";
            results.SetMessage(message).SetData(message);

            return context;
        }

        public static IActionResult FromModelState(ActionContext context)
        {
            var instance = new ValidationFailureResponse();
            instance.ParseFromModelState(context);
            return instance.GetResponseContext();
        }
    }
}
