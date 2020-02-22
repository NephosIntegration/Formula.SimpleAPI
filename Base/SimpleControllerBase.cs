using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Formula.SimpleCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Formula.SimpleAPI
{
    public abstract class SimpleControllerBase : Controller
    {
        protected StatusBuilder HandleModelState()
        {
            var results = new StatusBuilder();

            if (ModelState.IsValid == false) 
            {
                var errors = ModelState.Keys.Select(e => "" + e + "");

                foreach(var modelStatekey in ModelState.Keys)
                {
                    var entry = ModelState[modelStatekey];
                    if (entry.ValidationState == ModelValidationState.Invalid)
                    {
                        foreach(var err in entry.Errors)
                        {
                            results.RecordFailure(err.ErrorMessage, modelStatekey);
                        }
                    }
                }

                results.SetMessage("Invalid Data");
            } 

            return results;          
        }

        protected List<string> GetErrors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();

            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }

    }
}