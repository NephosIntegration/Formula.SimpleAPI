using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Formula.SimpleRepo;
using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ResourceControllerBase<TController, TModel, TRepository> : ReadOnlyResourceControllerBase<TController, TModel, TRepository>
        where TController : class
        where TModel : new()
        where TRepository : IRepository<TModel>
    {
        public ResourceControllerBase(ILogger<TController> logger, TRepository repository) : base(logger, repository)
        {
        }

        // Create a new resource
        [HttpPost]
        public virtual async Task<Status<TModel>> Post(TModel model)
        {
            var output = new Status<TModel>();
            try
            {
                var id = await _repository.InsertAsync(model);
                var results = await _repository.GetAsync(id.Value);
                output.SetData(results);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        // Updates the resource identified by id
        [HttpPut]
        public virtual async Task<Status<TModel>> Put(String id, TModel model)
        {
            var output = new Status<TModel>();
            try
            {
                var recordsUpdated = await _repository.UpdateAsync(model);
                var results = await _repository.GetAsync((object)id);
                output.SetData(results);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        // Update a specific attribute on a resource
        [HttpPatch("{id}")]
        public virtual async Task<Status<TModel>> Patch(String id, PatchModel model)
        {
            var output = new Status<TModel>();
            try
            {
                var results = await _repository.GetAsync((object)id);
                output.SetData(results);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        // Delete removes resource
        [HttpDelete("{id}")]
        public virtual async Task<Status<TModel>> Delete(String id)
        {
            var output = new Status<TModel>();
            try
            {
                var recordsUpdated = await _repository.DeleteAsync((object)id);
                var results = await _repository.GetAsync((object)id);
                output.SetData(results);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }
    }
}