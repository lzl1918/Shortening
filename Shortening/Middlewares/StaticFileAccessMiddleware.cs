using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shortening.Middlewares
{
    public sealed class StaticFileAccessMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostingEnvironment env;

        public StaticFileAccessMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            this.next = next;
            this.env = env;
        }

        public Task Invoke(HttpContext context)
        {
            if (!IsRequestHandled(context.Request))
                return next(context);

            string requestPath = context.Request.Path.Value;
            string fullPath = Path.Combine(env.WebRootPath, requestPath.Substring(1));
            IFileInfo fileInfo = env.WebRootFileProvider.GetFileInfo(requestPath);
            if (!fileInfo.Exists)
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            string extension = Path.GetExtension(requestPath);
            context.Response.ContentType = GetContentType(extension);
            context.Response.ContentLength = fileInfo.Length;
            return context.Response.SendFileAsync(fileInfo);
        }

        private bool IsRequestHandled(HttpRequest request)
        {
            if (request.Method != "GET")
                return false;

            string path = request.Path.Value;
            if (path == "/site.ico")
                return true;
            if (path.StartsWith("/css") && path.EndsWith(".css"))
                return true;
            if (path.StartsWith("/js") && path.EndsWith(".js"))
                return true;
            return false;
        }
        private string GetContentType(string extension)
        {
            if (extension == ".css")
                return "text/css";
            if (extension == ".js")
                return "application/x-javascript";
            if (extension == ".ico")
                return "image/x-icon";
            if (extension == ".png")
                return "image/png";
            if (extension == ".jpg")
                return "image/jpeg";
            if (extension == ".jpeg")
                return "image/jpeg";
            return "application/octet-stream";
        }
    }
}
