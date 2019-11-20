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

        [HttpGet]
        public virtual Task<IEnumerable<TModel>> GetList()
        {
            return _repository.GetListAsync();
        }

        [HttpGet("{id}")]
        public Task<TModel> Get(int id) 
        {
            return _repository.GetAsync(id);
        }

        [HttpGet("query/{constraints}")]
        public Task<IEnumerable<TModel>> Query(String constraints)
        {
            var bindable = _repository.WhereFromJson(constraints);
            return _repository.GetListAsync(bindable.Sql, bindable.Parameters);
        }

    }
}