using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data;
using Casperinc.OpenCleveland.DocumentParser.Core.Helpers;
using Casperinc.OpenCleveland.DocumentParser.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Casperinc.OpenCleveland.DocumentParser.Facade
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDocumentSource>(
                new DbDocumentSource(Configuration["Data:ConnectionStrings:MySQL"])
            );
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Mapper.Initialize(cfg => {
                cfg.AddProfile<DataDTOMapperProfile>();
            });

            app.UseMvc();
        }
    }
}
