using MediatR;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestSharp;
using WebApiEstimadorDeIngresos.Dto;
using WebApiEstimadorDeIngresos.Model;

namespace WebApiEstimadorDeIngresos.Aplication
{
    public class EstimadorDeIngresos
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

                var retryCount = ConfigurationEstimadorIngresos.AppsettingsEstimador["retryCount:value"];
                var value = Int32.Parse(retryCount);


                //Se realizará el reintento cuando ocurra un InternalServerError (código 500) y un RequestTimeout (código 408)

                policyRetries = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: value, //Definimos el numero de reintentos
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

                ResponseDto response = new ResponseDto();

                var endpoint = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:EndPoint"];
                var query = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:Estimador"];


                var state = false;
                var resp = new RestSharp.RestResponse();

                try
                {

                    _logger.LogInformation(

                        "\n **** ------------------------------ **** \n" +

                            $"    RUN WebApiEstimadorDeIngresos \n" +
                            $"    Endpoint: {endpoint}/graphql/ \n" +
                            $"    Type: Get \n" +
                            $"    IdCliente: {request?.id_cliente} \n" +
                            $"    WebApi-local: api/servicios-credito/estimador-ingresos \n" +

                        " **** ------------------------------ ****  "
                     );

                    var param = new RequestEstimadorIngresos();
                    param.Query = query;

                    var v = new Variables();
                    v.IdCliente = request?.id_cliente;

                    param.Variables = v;

                    var restClient = new RestClient(endpoint);

                    var requestEndPoint = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);

                    requestEndPoint.RequestFormat = DataFormat.Json;
                    requestEndPoint.AddHeader("Content-Type", "application/json");

                    if (request?.id_cliente == 0) //Pruebas unitarias
                    {
                        //#1: Habilitamos la bandera
                        state = false;

                        var paramTest = @"""
                                 {
                                    ""data"":[ {""idCliente"":12345} ]
                                 }
                                """;
                        requestEndPoint.AddBody(paramTest);

                    }
                    else if (request?.id_cliente == 1) //Pruebas unitarias
                    {
                        //#1: Habilitamos la bandera
                        state = true;

                        //#2: Creamos el Http Request Response
                        resp = new RestResponse
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            ContentType = "application/json",
                            ResponseUri = new Uri(endpoint+"/productizar/riesgosmodevaestimador/graphql"),
                            Content = "{'data':{'estimadorIngresosAll':{'totalCount':1,'estimadoringresos':[{'ingestionYear':2022.0,'ingestionMonth':4.0,'ingestionDay':11.0,'idCliente':'9056626','clasificacion':'ESTIMADOR FIJO','ingFinal':383.9102735153108,'antiguedadLaboralFinal':999.0,'lugarTrabajoFinal':'0','ingresoSolicitudes':null,'antiguedadLaboralSolicitudes':null,'lugarTrabajoSolicitudes':null,'flagSolicitudes':'NO','ingPlanilla':383.9102735153108,'antiguedadLaboralPlanilla':999.0,'lugarTrabajoPlanilla':'0','activo':1.0,'migrar':1.0}]}}}"
                        };

                    }
                    else
                    {
                        requestEndPoint.AddBody(param);


                        //Polly
                        var policyResult = await policyRetries.ExecuteAndCaptureAsync(async () =>
                        {

                            resp = await restClient.ExecuteAsync(requestEndPoint, cancellationToken);

                            HttpResponseMessage responseMsg = new HttpResponseMessage(resp.StatusCode);
                            return responseMsg;

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



                        var responseUri = resp.ResponseUri;
                        var errorMessage = resp.ErrorMessage;
                        var errorException = resp.ErrorException;
                        var responseStatus = resp.ResponseStatus;
                        var statusCode = resp.StatusCode;
                        var content = resp.Content;

                        _logger.LogInformation(

                            "\n **** ------------------------------ **** \n" +

                                $"    Response \n" +
                                $"    ResponseUri: {responseUri}\n" +
                                $"    ResponseStatus: {responseStatus}\n" +
                                $"    StatusCode: {statusCode} \n" +
                                $"    ErrorMessage: {errorMessage} \n" +
                                $"    ErrorException: {errorException} \n" +
                                $"    Content: {content} \n" +

                            " **** ------------------------------ ****  "
                         );


                        state = resp.IsSuccessful;
                    }




                    if (state)
                    {
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


                        var data = resp.Content;


                        if(data != null )
                        {
                            var obj = JsonConvert.DeserializeObject<ResponseEstimadorIngresos>(data);


                            

                            obj?.Data?.EstimadorIngresosAll?.Estimadoringresos?.ForEach(estimador =>
                            {
                                response.Estimadoringresos = new List<EstimadoringresosItem>();

                                response.Estimadoringresos.Add(new EstimadoringresosItem
                                {
                                    IngestionYear = estimador.IngestionYear,
                                    IngestionMonth = estimador.IngestionMonth,
                                    IngestionDay = estimador.IngestionDay,
                                    IdCliente = estimador.IdCliente,
                                    Clasificacion = estimador.Clasificacion,
                                    IngFinal = estimador.IngFinal,
                                    AntiguedadLaboralFinal = estimador.AntiguedadLaboralFinal,
                                    LugarTrabajoFinal = estimador.LugarTrabajoFinal,
                                    IngresoSolicitudes = estimador.IngresoSolicitudes,
                                    AntiguedadLaboralSolicitudes = estimador.AntiguedadLaboralSolicitudes,
                                    LugarTrabajoSolicitudes = estimador.LugarTrabajoSolicitudes,
                                    FlagSolicitudes = estimador.FlagSolicitudes,
                                    IngPlanilla = estimador.IngPlanilla,
                                    AntiguedadLaboralPlanilla = estimador.AntiguedadLaboralPlanilla,
                                    LugarTrabajoPlanilla = estimador.LugarTrabajoPlanilla,
                                    Activo = estimador.Activo,
                                    Migrar = estimador.Migrar,

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
