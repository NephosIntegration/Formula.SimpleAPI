using System;
using System.Linq;
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
    public abstract class ReadOnlyResourceControllerBase<TController, TModel, TRepository> : Controller, IResourceController<TController, TModel, TRepository>
        where TController : class
        where TModel : new()
        where TRepository : IReadOnlyRepository<TModel>
    {
        protected readonly ILogger<TController> _logger;

        protected readonly TRepository _repository;

        public ReadOnlyResourceControllerBase(ILogger<TController> logger, TRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("query/{constraints}")]
        public virtual async Task<StatusBuilder> QueryAsync(String constraints)
        {
            var output = new StatusBuilder();
            try
            {
                var results = await _repository.GetAsync(constraints);
                output.SetData(results);
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        // Gets a specific resource by id
        [HttpGet("{id}")]
        public virtual async Task<StatusBuilder> Get(String id) 
        {
            var output = new StatusBuilder();
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

        // Gets all resources
        [HttpGet]
        public virtual async Task<StatusBuilder> GetList()
        {
            var output = new StatusBuilder();
            try
            {
                var results = await _repository.GetAsync();
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