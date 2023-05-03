using Elasticsearch.Net;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace WebApiEstimadorDeIngresos.Helpers
{
    public static class Logger
    {
        public static void ConfigureLogger(HostBuilderContext context, IServiceProvider services,
        LoggerConfiguration configuration)
        {
            ElasticsearchSinkOptions options =
                new ElasticsearchSinkOptions(new Uri(context.Configuration.GetValue<string>("Elasticsearch:ServerUrl")))
                {
                    IndexFormat = context.Configuration.GetValue<string>("Elasticsearch:IndexFormat"),
                    TemplateName = context.Configuration.GetValue<string>("Elasticsearch:TemplateName"),
                    AutoRegisterTemplate = true,
                    BatchAction = ElasticOpType.Create,
                    TypeName = null
                };

            if (!string.IsNullOrEmpty(context.Configuration.GetValue<string>("Elasticsearch:ApiKey")))
            {
                options.ModifyConnectionSettings = c =>
                    c.ApiKeyAuthentication(
                            new ApiKeyAuthenticationCredentials(
                                context.Configuration.GetValue<string>("Elasticsearch:ApiKey")))
                        .ServerCertificateValidationCallback((sender, cert, chain, error) => { return true; })
                        .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
            }

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Elasticsearch(options);
        }

    }
}
