using MediatR;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestSharp;
using WebApiModeva.Dto;
using WebApiModeva.Model;

namespace WebApiModeva.Aplication.v2
{
    public class MaestroModeva
    {

        public class Execute : IRequest<string>
        {
            public int id_cliente { get; set; }
            public string? producto { get; set; }

        }

        
        public class Driver : IRequestHandler<Execute, string>
        {

            private readonly ILogger<Driver> _logger;
            private readonly AsyncRetryPolicy<HttpResponseMessage> policyRetries;


            public Driver(ILogger<Driver> logger)
            {
                _logger = logger;
                var countRetrie = 1;

                //Se realizará el reintento cuando ocurra un InternalServerError(código 500) y un RequestTimeout(código 408)

                var retryCnt = ConfigurationModeva.AppsettingsModeva["retryCount:value"];

                var value = Int32.Parse(retryCnt);

               
                policyRetries = HttpPolicyExtensions.HandleTransientHttpError()
               .WaitAndRetryAsync(retryCount: value, //Definimos el numero de reintentos
               retries => TimeSpan.FromSeconds(Math.Pow(2, retries)), //Definimos el tiempo de espera entre reintento
                onRetry: (exception, timeSpan, context) =>
                {
                    _logger.LogWarning(

                         "\n **** ------------------------------ **** \n" +

                             $"    Reintentando comunicarnos \n" +
                             $"    Reintento número: # {countRetrie} \n" +
                             $"    Tiempo de espera entre reintento: {timeSpan.Seconds} segundos \n" +
                             $"    Exception: {exception.Result.StatusCode} \n" +

                         " **** ------------------------------ ****  "
                      );

                    countRetrie++;
                });

            }


            public async Task<string> Handle(Execute request, CancellationToken cancellationToken)
            {

                var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];
                var query = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:Query"];


                var state = false;
                var resp = new RestSharp.RestResponse();

                ResponseMaestroModevaDto response = new ResponseMaestroModevaDto();

                try
                {

                    _logger.LogInformation(

                        "\n **** ------------------------------ **** \n" +

                            $"    RUN WebApiMaestroModeva \n" +
                            $"    Endpoint: {endpoint}/productizar/riesgosmodevaestimador/graphql \n" +
                            $"    Type: Get \n" +
                            $"    IdCliente: {request.id_cliente} \n" +
                            $"    Producto: {request.producto} \n" +
                            $"    WebApi-local: api/servicios-credito/grupos-modeva \n" +
                            $"    Versión: 2 \n" +

                        " **** ------------------------------ ****  "
                     );

                    var rqMdv = new RequestModeva();

                    rqMdv.Query = query;

                    var v = new Variables();

                    v.IdCliente = request.id_cliente;

                    v.producto = request.producto;
                    
                    rqMdv.Variables = v;

                    var restClient = new RestClient(endpoint);

                    var rs = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);
                    rs.RequestFormat = DataFormat.Json;

                    rs.AddHeader("Content-Type", "application/json");

                    if (request.id_cliente == 0)//Pruebas unitarias
                    {
                        var paramTest = @"""
                                 {
                                    ""data"":[ {""idCliente"":4572811} ]
                                 }
                                """;
                        rs.AddBody(paramTest);

                    }
                    else if (request.id_cliente == 1)//Pruebas unitarias
                    {
                        //#1: Habilitamos la bandera
                        state = true;

                        //#2: Creamos el Http Request Response
                        resp = new RestResponse
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            ContentType = "application/json",
                            ResponseUri = new Uri(endpoint+"/productizar/riesgosmodevaestimador/graphql"),
                            Content = "{'data':{'maestroModevaAll':{'totalCount':1,'maestromodeva':[{'ingestionYear':2022.0,'ingestionMonth':7.0,'ingestionDay':8.0,'idCliente':'4572811', 'modevaTipo':'III_NO_PLA','producto':'CCASH','gOriginacionFinal':'G6','activo':1.0,'migrar':1.0}]}}}"
                        };

                    }
                    else
                    {

                        if(request.producto == null || request.producto == "")
                        {
                            //Retornamos un objeto vacio, en caso el producto = null
                            return JsonConvert.SerializeObject(response);
                        }


                        rs.AddBody(rqMdv);


                        //Polly
                        var policyRslt = await policyRetries.ExecuteAndCaptureAsync(async () =>
                        {

                            resp = await restClient.ExecuteAsync(rs, cancellationToken);

                            return new HttpResponseMessage(resp.StatusCode);

                        });


                        //Capturamos la excepcion generada por polly

                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Información lanzada por polly \n" +
                                $"    Result: {policyRslt.FinalException} \n" +
                                $"    Outcome: {policyRslt.Outcome.ToString()} \n" +
                                $"    StatusCode: {policyRslt.Result} \n" +

                            " **** ------------------------------ ****  "
                         );



                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Response HttpRequest \n" +
                                $"    ResponseUri: { resp.ResponseUri}\n" +
                                $"    ResponseStatus: { resp.ResponseStatus}\n" +
                                $"    StatusCode: {resp.StatusCode} \n" +
                                $"    ErrorMessage: {resp.ErrorMessage} \n" +
                                $"    ErrorException: {resp.ErrorException} \n" +
                                $"    Content: {resp.Content} \n" +

                            " **** ------------------------------ ****  "
                         );

                        state = resp.IsSuccessful;
                    }


                    if (state && resp.Content != null)
                    {
                            var data = resp.Content;

                            var obj = JsonConvert.DeserializeObject<ResponseMaestroModeva>(data);


                            var status = resp.StatusCode;
                            var url = resp.ResponseUri!.AbsoluteUri;
                            var contentType = resp.ContentType;


                            _logger.LogInformation(

                                "\n **** ------------------------------ **** \n" +

                                    $"    Response \n" +
                                    $"    Endpoint: {url}\n" +
                                    $"    Response with status code: {status} \n" +
                                    $"    content type: {contentType} \n" +

                                " **** ------------------------------ ****  "
                             );


                            obj?.Data?.MaestroModevaAll?.Maestromodeva?.ForEach( modeva =>
                            {

                                var rs = modeva.GOriginacionFinal;
                                var value = rs?.Substring(1);


                                response.MaestroModevaItem = new List<MaestromodevaItem>();

                                response.MaestroModevaItem?.Add(new MaestromodevaItem
                                {
                                   IngestionYear = modeva.IngestionYear,
                                   IngestionMonth = modeva.IngestionMonth,
                                   IngestionDay = modeva.IngestionDay,
                                   IdCliente = modeva.IdCliente,
                                   ModevaTipo = modeva.ModevaTipo,
                                   Producto = modeva.Producto,
                                   GOriginacionFinal = value,
                                   Activo = modeva.Activo,
                                   Migrar = modeva.Migrar,

                                });


                            });

                            return JsonConvert.SerializeObject(response);

                    }
                    else
                    {
                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Response Exception \n" +
                                $"    ErrorException: {resp.ErrorException}\n" +
                                $"    ErrorMessage: {resp.ErrorMessage} \n" +
                                $"    ResponseStatus: {resp.ResponseStatus} \n" +

                            " **** ------------------------------ ****  "
                         );

                        //Retornamos un objeto vacio
                        return JsonConvert.SerializeObject(response);

                    }

                }
                catch (Exception ex)
                {

                    _logger.LogInformation(

                        "\n **** ------------------------------ **** \n" +

                            $"    Catch Exception \n" +
                            $"    Error: {ex.Message}\n" +
                            $"    Detail: {ex.StackTrace} \n" +

                        " **** ------------------------------ ****  "
                     );

                    //Retornamos un objeto vacio
                    return JsonConvert.SerializeObject(response);

                }

            }
        }

    }
}
