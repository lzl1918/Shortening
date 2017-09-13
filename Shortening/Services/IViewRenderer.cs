using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shortening.Services
{
    public interface IViewRenderer
    {
        string Render(string viewPath, HttpContext context, IReadOnlyDictionary<string, object> data, bool isMainPage);
    }

    public sealed class ViewRenderer : IViewRenderer
    {
        private readonly IRazorViewEngine viewEngine;
        private readonly IHostingEnvironment env;

        public ViewRenderer(IRazorViewEngine viewEngine, IHostingEnvironment env)
        {
            this.viewEngine = viewEngine;
            this.env = env;
        }

        public string Render(string viewPath, HttpContext context, IReadOnlyDictionary<string, object> data, bool isMainPage)
        {
            ViewEngineResult viewEngineResult = viewEngine.GetView(env.ContentRootPath, viewPath, isMainPage);
            if (!viewEngineResult.Success)
                throw new Exception($"Could not find view {viewPath}");
            IView view = viewEngineResult.View;
            using (StringWriter output = new StringWriter())
            {
                var viewContext = new ViewContext();
                viewContext.HttpContext = context;

                ViewDataDictionary viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                if (data != null)
                    foreach (var pair in data)
                        viewData[pair.Key] = pair.Value;

                viewContext.ViewData = viewData;
                viewContext.Writer = output;
                view.RenderAsync(viewContext).GetAwaiter().GetResult();
                return output.ToString();
            }
        }
    }
}
