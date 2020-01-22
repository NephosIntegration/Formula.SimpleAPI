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
    public abstract class ResourceControllerBase<TController, TModel, TRepository> : Controller, IResourceController<TController, TModel, TRepository>
        where TController : class
        where TModel : new()
        where TRepository : IRepository<TModel>
    {
        protected readonly ILogger<TController> _logger;

        protected readonly TRepository _repository;

        public ResourceControllerBase(ILogger<TController> logger, TRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("query/{constraints}")]
        public virtual Task<IEnumerable<TModel>> Query(String constraints)
        {
            var bindable = _repository.WhereFromJson(constraints);
            return _repository.GetListAsync(bindable.Sql, bindable.Parameters);
        }

        // Gets a specific resource by id
        [HttpGet("{id}")]
        public virtual Task<TModel> Get(int id) 
        {
            return _repository.GetAsync(id);
        }

        // Gets all resources
        [HttpGet]
        public virtual Task<IEnumerable<TModel>> GetList()
        {
            return _repository.GetListAsync();
        }

        // Create a new resource
        [HttpPost]
        public virtual async Task<TModel> Post(TModel model)
        {
            var id = await _repository.InsertAsync(model);
            return await _repository.GetAsync(id);
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
            throw new Exception("Not implemented yet");
            return await _repository.GetAsync(id);
        }

        // Delete removes resource
        [HttpDelete("{id}")]
        public virtual async Task<TModel> Delete(int id, TModel model)
        {
            var recordsUpdated = await _repository.DeleteAsync(model);
            return await _repository.GetAsync(id);
        }
    }
}