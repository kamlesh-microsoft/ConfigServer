﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace ConfigServer.Core.Internal
{
    internal interface IConfigHttpResponseFactory
    {
        Task BuildResponse(HttpContext context, object config);
    }

    internal class ConfigHttpResponseFactory : IConfigHttpResponseFactory
    {
        public Task BuildResponse(HttpContext context, object config)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(config, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
}