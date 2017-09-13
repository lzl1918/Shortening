using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shortening.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shortening.Middlewares;
using Shortening.Models;
using Microsoft.EntityFrameworkCore;

namespace Shortening
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();


        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IViewRenderer, ViewRenderer>();

            services.AddDbContext<MapDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("MappedItems"));
            }, ServiceLifetime.Singleton, ServiceLifetime.Singleton);

            services.AddSingleton<IAliasProvider, AliasProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<StaticFileAccessMiddleware>();
            app.UseMiddleware<AddEntryMiddleware>(Configuration["Host"]);
            app.UseMiddleware<MapValueMiddleware>();

            // 404
            app.Run((context) =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            });
        }
    }
}
