using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    public interface IResourceController<TController, TModel, TRepository>
    {
        [HttpGet]
        Task<StatusBuilder> GetList();

        [HttpGet("{id}")]
        Task<StatusBuilder> Get(object id);
    }
}