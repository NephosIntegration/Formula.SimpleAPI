using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Formula.SimpleRepo;

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
        public virtual async Task<TModel> Post(TModel model)
        {
            var id = await _repository.InsertAsync(model);
            return await _repository.GetAsync(id.Value);
        }

        // Updates the resource identified by id
        [HttpPut("{id}")]
        public virtual async Task<TModel> Put(int id, TModel model)
        {
            var recordsUpdated = await _repository.UpdateAsync(model);
            return await _repository.GetAsync(id);
        }

        // Update a specific attribute on a resource
        [HttpPatch("{id}")]
        public virtual async Task<TModel> Patch(int id, PatchModel model)
        {
            //throw new Exception("Not implemented yet");
            return await _repository.GetAsync(id);
        }

        // Delete removes resource
        [HttpDelete("{id}")]
        public virtual async Task<TModel> Delete(int id)
        {
            var recordsUpdated = await _repository.DeleteAsync(id);
            return await _repository.GetAsync(id);
        }
    }
}