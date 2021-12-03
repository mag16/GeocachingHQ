using Geocaches.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GeocachingApi {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {

            services.AddControllers ();
            services.AddDbContext<GeocachesContext> (options => {
                options.EnableDetailedErrors ();
                options.UseNpgsql (Configuration.GetConnectionString ("geocachinghq"));
            });

            services.AddDbContext<ItemContext> (options => {
                options.EnableDetailedErrors ();
                options.UseNpgsql (Configuration.GetConnectionString ("geocachinghq"));
            });

            // services.AddDbContext<GeocachesContext> (options =>
            //     options.UseNpgsql(Configuration.GetConnectionString("geocachinghq")));

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new OpenApiInfo { Title = "GeocachingApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
                app.UseSwagger ();
                app.UseSwaggerUI (c => c.SwaggerEndpoint ("/swagger/v1/swagger.json", "GeocachingApi v1"));
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                // endpoints.MapControllers();
                endpoints.MapControllerRoute ("geocaches", "api/geocaches/{id?}",
                    defaults : new { controller = "Geocaches", action = "Index" });

                //This method established yet another "convention" route, for the User/Index action.
                endpoints.MapControllerRoute ("items", "api/items/{id?}",
                    defaults : new { controller = "Items", action = "Index" });

            });

        }
    }
}