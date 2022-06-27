using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using TechParser.Core.Data;

namespace TechParser
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ParserDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection")));
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BonchChat",

                });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                c.IncludeXmlComments(Path.Combine(basePath, "Api.xml"));
                c.CustomSchemaIds(x => x.FullName);
            });
        }   


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Proj");
            });
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
     
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}