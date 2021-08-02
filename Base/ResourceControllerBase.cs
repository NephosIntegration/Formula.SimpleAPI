using Formula.SimpleCore;
using Formula.SimpleRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Formula.SimpleAPI
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ResourceControllerBase<TController, TModel, TConstraints, TRepository> : ReadOnlyResourceControllerBase<TController, TModel, TConstraints, TRepository>
        where TController : class
        where TModel : new()
        where TConstraints : new()
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
        public virtual async Task<Status<TModel>> Put(string id, TModel model)
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
        public virtual async Task<Status<TModel>> Patch(string id, PatchModel model)
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
        public virtual async Task<Status<TModel>> Delete(string id)
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