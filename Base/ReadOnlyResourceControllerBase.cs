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
                var bindable = _repository.WhereFromJson(constraints);
                var results = await _repository.GetListAsync(bindable.Sql, bindable.Parameters);
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
        public virtual async Task<StatusBuilder> Get(int id) 
        {
            var output = new StatusBuilder();
            try
            {
                var results = await _repository.GetAsync(id);
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
                var results = await _repository.GetListAsync();
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