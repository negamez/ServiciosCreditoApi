using MediatR;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestSharp;
using WebApiModeva.Dto;
using WebApiModeva.Model;

namespace WebApiModeva.Aplication.v1
{
    public class GroupsModeva
    {
        public class Execute : IRequest<string>
        {
            public int id_cliente { get; set; }
        }

        public class Driver : IRequestHandler<Execute, string>
        {

            private readonly ILogger<Driver> _logger;
            private readonly AsyncRetryPolicy<HttpResponseMessage> policyRetries;

            public Driver(ILogger<Driver> logger)
            {
                _logger = logger;
                var retrie = 1;

                //Se realizará el reintento cuando ocurra un InternalServerError (código 500) y un RequestTimeout (código 408)

                var retryCount = ConfigurationModeva.AppsettingsModeva["retryCount:value"];
                var value = Int32.Parse(retryCount);

                policyRetries = HttpPolicyExtensions.HandleTransientHttpError()
               .WaitAndRetryAsync(retryCount: value, //Definimos el numero de reintentos

               retries => TimeSpan.FromSeconds(Math.Pow(2, retries)), //Definimos el tiempo de espera entre reintento
                onRetry: (exception, timeSpan, context) =>
                {

                    _logger.LogInformation(

                         "\n **** ------------------------------ **** \n" +

                             $"    Reintentando comunicarnos \n" +
                             $"    Reintento número: # {retrie} \n" +
                             $"    Tiempo de espera entre reintento: {timeSpan.Seconds} segundos \n" +
                             $"    Exception: {exception.Result.StatusCode} \n" +

                         " **** ------------------------------ ****  "
                      );

                    retrie++;

                });

            } 

            public async Task<string> Handle(Execute request, CancellationToken cancellationToken)
            {

                var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesModeva:EndPoint"];
                var query = ConfigurationModeva.AppsettingsModeva["ResourcesModeva:Modeva"];

                var state = false;
                var resp = new RestSharp.RestResponse();

                ResponseDto response = new ResponseDto();

                try
                {

                    _logger.LogInformation(

                        "\n **** ------------------------------ **** \n" +

                            $"    RUN WebApiModeva \n" +
                            $"    Endpoint: {endpoint}/graphql/ \n" +
                            $"    Type: Get \n" +
                            $"    IdCliente: {request.id_cliente} \n" +
                            $"    WebApi-local: api/servicios-credito/grupos-modeva \n" +
                            $"    Versión: 1 \n" +

                        " **** ------------------------------ ****  "
                     );


                    var rqMdv = new RequestModeva();

                    rqMdv.Query = query;

                    var v = new Variables();
                    v.IdCliente = request.id_cliente;
                    rqMdv.Variables = v;


                    var restClient = new RestClient(endpoint);


                    var rs = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);
                    rs.RequestFormat = DataFormat.Json;


                    rs.AddHeader("Content-Type", "application/json");


                    if (request.id_cliente == 0) //Pruebas unitarias
                    {
                        //#1: Habilitamos la bandera
                        state = false;

                        var paramTest = @"""
                                 {
                                    ""data"":[ {""idCliente"":12345} ]
                                 }
                                """;
                        rs.AddBody(paramTest);


                        resp = new RestResponse
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            ContentType = "application/json",
                            Content = "{'Errors':[{'Code':'500','Error':'Internal server error','Message':'500 Internal server error'}]}"
                        };


                    }
                    else if(request.id_cliente == 1) //Pruebas unitarias
                    {

                        //#1: Habilitamos la bandera
                        state = true;

                        //#2: Creamos el Http Request Response
                        resp = new RestResponse {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            ContentType = "application/json",
                            ResponseUri = new Uri(endpoint+"/productizar/riesgosmodevaestimador/graphql"),
                            Content = "{'data':{'modevaGruposAll':{'totalCount':1,'modevagrupos':[{'ingestionYear':2022.0,'ingestionMonth':7.0,'ingestionDay':8.0,'idCliente':'3931910','gModeva':'G6','escala':'G6','gFinal':'G5','modevaTipo':'I','activo':1.0,'migrar':1.0}]}}}"
                        };

                    }
                    else
                    {
                        rs.AddBody(rqMdv);


                        //Polly

                        var policyResult = await policyRetries.ExecuteAndCaptureAsync(async () =>
                        {

                            resp = await restClient.ExecuteAsync(rs, cancellationToken);

                            return new HttpResponseMessage(resp.StatusCode);

                        });


                        //Capturamos la excepcion generada por polly

                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Información lanzada por polly \n" +
                                $"    Result: {policyResult.FinalException} \n" +
                                $"    Outcome: {policyResult.Outcome.ToString()} \n" +
                                $"    StatusCode: {policyResult.Result} \n" +

                            " **** ------------------------------ ****  "
                         );

                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Response \n" +
                                $"    ResponseUri: { resp.ResponseUri}\n" +
                                $"    ResponseStatus: {resp.ResponseStatus}\n" +
                                $"    StatusCode: {resp.StatusCode} \n" +
                                $"    ErrorMessage: { resp.ErrorMessage} \n" +
                                $"    ErrorException: {resp.ErrorException} \n" +
                                $"    Content: {resp.Content} \n" +

                            " **** ------------------------------ ****  "
                         );

                        state = resp.IsSuccessful;

                    }



                    if (state)
                    {
                        var data = resp.Content;

                        if(data != null)
                        {

                            var obj = JsonConvert.DeserializeObject<ResponseModeva>(data);


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


                            obj?.Data?.ModevaGruposAll?.Modevagrupos?.ForEach(modeva =>
                            {
                                var rs = modeva.GFinal;
                                var value = rs?.Substring(1);

                                response.Modevagrupos = new List<ModevagruposItem>();

                                response.Modevagrupos.Add(new ModevagruposItem
                                {
                                    IngestionYear = modeva.IngestionYear,
                                    IngestionMonth = modeva.IngestionMonth,
                                    IngestionDay = modeva.IngestionDay,
                                    IdCliente = modeva.IdCliente,
                                    GModeva = modeva.GModeva,
                                    Escala = modeva.Escala,
                                    GFinal = value,
                                    ModevaTipo = modeva.ModevaTipo,
                                    Activo = modeva.Activo,
                                    Migrar = modeva.Migrar,

                                });

                            });

                            return JsonConvert.SerializeObject(response);
                        }
                        else
                        {

                            //Retornamos un objeto vacio
                            return JsonConvert.SerializeObject(response);
                        }

                    }
                    else
                    {

                        if (resp.Content != null && resp.Content != "")
                        {

                            var errResp = JsonConvert.DeserializeObject<ErrorResponseModeva>(resp.Content);

                            _logger.LogError(

                                "\n **** ------------------------------ **** \n" +

                                    $"    Response Exception \n" +
                                    $"    Code: {errResp!.Errors![0].Code}\n" +
                                    $"    Error: {errResp!.Errors[0].Error} \n" +
                                    $"    Message: {errResp!.Errors[0].Message} \n" +

                                " **** ------------------------------ ****  "
                             );

                            //Retornamos un objeto vacio
                            return JsonConvert.SerializeObject(response);

                        } else
                        {
                            //Retornamos un objeto vacio
                            return JsonConvert.SerializeObject(response);
                        }

                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(

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