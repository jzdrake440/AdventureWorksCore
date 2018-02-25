using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using AdventureWorks.BLL.Services;
using AdventureWorks.BLL.Utility;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace AdventureWorks
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            
            services.AddAutoMapper();

            services.AddDbContext<AdventureWorks2017Context>(
                options => options.UseSqlServer(Configuration.GetConnectionString("AdventureWorks2017")));

            services.AddPpeService();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IDataTableService, DataTableService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            //handler routing of static resources
            //currently, only handling images; css and js are bundled into wwwroot
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "resources", "img")),
                RequestPath = "/img"
            });
            app.UseMvc();
        }
    }
}
