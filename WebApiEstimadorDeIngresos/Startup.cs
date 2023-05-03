using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WebApiEstimadorDeIngresos
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
            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true; //Proporciona las diferentes versiones del api que estan disponibles para el cliente
                options.AssumeDefaultVersionWhenUnspecified = true; //Esta configuración permitirá que la api tome automáticamente api_verions = 1.0, en caso de que no se haya especificado
                options.DefaultApiVersion = Microsoft.AspNetCore.Mvc.ApiVersion.Default;//Damos la versión por defecto de la api 1.0
                options.ApiVersionReader = new HeaderApiVersionReader("version");//Establecemos que por medio de la Key "version" consumiremos un endpoint en especifico
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
            //Mediador busca todos los IRequest y IRequestHandlers que nuestro proyecto tenga.
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
                    options.DefaultModelsExpandDepth(-1);
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
