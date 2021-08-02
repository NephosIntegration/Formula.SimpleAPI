using Formula.SimpleCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Formula.SimpleAPI
{
    public interface IResourceController<TController, TModel, TRepository>
    {
        [HttpGet("query/{constraints}")]
        Task<Status<List<TModel>>> QueryAsync(string constraints);

        [HttpGet]
        Task<Status<List<TModel>>> GetList();

        [HttpGet("{id}")]
        Task<Status<TModel>> Get(string id);
    }
}