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
    }
}