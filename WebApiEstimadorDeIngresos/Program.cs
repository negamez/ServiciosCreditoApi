using Elastic.Apm.NetCoreAll;
using Serilog;
using WebApiEstimadorDeIngresos;
using WebApiEstimadorDeIngresos.Helpers;

var builder = WebApplication.CreateBuilder(args);

//Hsts
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromSeconds(31536000);
});

// Add services to the container.
builder.Host.UseSerilog(Logger.ConfigureLogger);
builder.Host.UseAllElasticApm();

var startup = new Startup(builder.Configuration);
startup.ConfigureService(builder.Services);


var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHsts();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Content-Security-Policy", "script-src https://stackpath.bootstrapcdn.com/  'self' 'unsafe-inline';style-src https://stackpath.bootstrapcdn.com/ 'self' 'unsafe-inline';");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block ");
    context.Response.Headers.Add("X-Frame-Options", "deny");
    context.Response.Headers.Add("Expect-Ct", "max-age=604800,enforce");

    await next.Invoke();
});

app.UseHttpsRedirection();

startup.Configure(app, app.Environment);

app.Run();
