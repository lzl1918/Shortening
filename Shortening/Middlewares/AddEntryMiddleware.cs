using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shortening.Models;
using Shortening.Services;

namespace Shortening.Middlewares
{
    public sealed class AddEntryMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MapDbContext dbcontext;
        private readonly IAliasProvider aliasProvider;
        private readonly IViewRenderer viewRenderer;

        private readonly string host;

        public AddEntryMiddleware(string host, RequestDelegate next, MapDbContext dbcontext, IAliasProvider aliasProvider, IViewRenderer viewRenderer)
        {
            this.host = host;
            this.next = next;
            this.dbcontext = dbcontext;
            this.aliasProvider = aliasProvider;
            this.viewRenderer = viewRenderer;
        }


        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/add.php"))
                return next(context);

            if (context.Request.Method == "GET")
                return TryGetAddPage(context);

            if (context.Request.Method == "POST")
                return TryAddEntry(context);

            context.Response.StatusCode = 400;
            return Task.CompletedTask;
        }

        private Task TryAddEntry(HttpContext context)
        {
            if (!(context.Request.Form.TryGetValue("value", out StringValues values) && values.Count > 0))
                return TryGetAddPage(context);

            string value = values[0].Trim();
            if (value.Length <= 0)
                return TryGetAddPage(context);

            MappedItem item = new MappedItem()
            {
                Alias = aliasProvider.GetAlias(),
                CreatedTime = DateTime.Now,
                Value = value
            };
            dbcontext.Items.Add(item);
            dbcontext.SaveChanges();
            context.Response.StatusCode = 200;
            return TryGetResultPage(context, item.Alias);
        }
        private Task TryGetAddPage(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            return context.Response.WriteAsync(viewRenderer.Render("Views/add.cshtml", context, null, true));
        }
        private Task TryGetResultPage(HttpContext context, string alias)
        {
            context.Response.ContentType = "text/html";
            return context.Response.WriteAsync(viewRenderer.Render("Views/result.cshtml", context, new Dictionary<string, object>()
            {
                ["result"] = host + "/" + alias
            }, true));
        }
    }
}
