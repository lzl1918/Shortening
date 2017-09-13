using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shortening.Models;
using Shortening.Services;

namespace Shortening.Middlewares
{
    public class MapValueMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MapDbContext dbcontext;

        public MapValueMiddleware(RequestDelegate next, MapDbContext dbcontext)
        {
            this.next = next;
            this.dbcontext = dbcontext;
        }


        public Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "GET")
                return next(context);

            string alias = context.Request.Path.Value.Trim().Substring(1).ToLower();
            if (!TryFindValue(alias, out string value))
                return next(context);
            context.Response.Redirect(value, true);
            return Task.CompletedTask;
        }

        private bool TryFindValue(string alias, out string value)
        {
            MappedItem item = dbcontext.Items.FirstOrDefault(i => i.Alias == alias);
            if (item == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = item.Value;
                return true;
            }
        }
    }
}
