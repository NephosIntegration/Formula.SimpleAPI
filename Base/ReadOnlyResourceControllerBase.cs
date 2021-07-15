using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Formula.SimpleRepo;
using Formula.SimpleCore;
using Newtonsoft.Json;

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

        [HttpPost("query")]
        public virtual async Task<Status<List<TModel>>> QueryAsync(TModel constraints)
        {
            var output = new Status<List<TModel>>();
            try
            {
                var serialized = JsonConvert.SerializeObject(
                    constraints,
                    Newtonsoft.Json.Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                );
                
                var results = await _repository.GetAsync(serialized);
                output.SetData(results.ToList());
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        [HttpGet("query/{constraints}")]
        public virtual async Task<Status<List<TModel>>> QueryAsync(String constraints)
        {
            var output = new Status<List<TModel>>();
            try
            {
                var results = await _repository.GetAsync(constraints);
                output.SetData(results.ToList());
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }

        // Gets a specific resource by id
        [HttpGet("{id}")]
        public virtual async Task<Status<TModel>> Get(String id) 
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

        // Gets all resources
        [HttpGet]
        public virtual async Task<Status<List<TModel>>> GetList()
        {
            var output = new Status<List<TModel>>();
            try
            {
                var results = await _repository.GetAsync();
                output.SetData(results.ToList());
            }
            catch (Exception ex)
            {
                output.RecordFailure(ex.Message);
            }
            return output;
        }
    }
}