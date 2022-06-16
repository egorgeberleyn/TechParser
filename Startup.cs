using Microsoft.EntityFrameworkCore;
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
        }   


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
     
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}