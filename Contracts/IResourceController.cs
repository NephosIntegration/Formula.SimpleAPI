using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    public interface IResourceController<TController, TModel, TRepository>
    {
        [HttpGet("query/{constraints}")]
        Task<Status<List<TModel>>> QueryAsync(String constraints);

        [HttpGet]
        Task<Status<List<TModel>>> GetList();

        [HttpGet("{id}")]
        Task<Status<TModel>> Get(String id);
    }
}