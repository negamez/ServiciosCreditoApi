using MediatR;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace WebApiModeva
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureService(IServiceCollection services)
        {

            // Add services to the container.
            services.AddControllers();

            services.AddEndpointsApiExplorer();

            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = Microsoft.AspNetCore.Mvc.ApiVersion.Default;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("version");
            });

            //Add ApiExplorer to discover versions
            services.AddVersionedApiExplorer(setup =>
            {
              setup.GroupNameFormat = "'v'VVV";
              setup.SubstituteApiVersionInUrl = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            //Mediador, busca todos los IRequest y IRequestHandlers que nuestro proyecto tenga.
            services.AddMediatR(Assembly.GetExecutingAssembly());

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Obtenemos las versiones de las API´s
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.DefaultModelsExpandDepth(-1); //Removemos la opcioin de esquemas

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                      options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description!.GroupName.ToUpperInvariant());
                    }

                });

            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }

    }
}
