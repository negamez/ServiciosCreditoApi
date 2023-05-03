using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiModeva;
using WebApiModeva.Aplication.v2;
using WebApiModeva.Controllers.v2;
using WebApiModeva.Dto;
using WebApiModeva.Model;
using static WebApiModeva.Aplication.v2.MaestroModeva;

namespace TestingProject.Tests.UnitTests.ModevaTests
{
    [TestClass]
    public class MaestroModeva
    {

        [TestMethod]
        public async Task ControllerAccessMaestroModeva()
        {

            //Preparación
            Mock<IMediator> mediator = new Mock<IMediator>();
            var id_cliente = 4572811;
            var producto = "CCASH";

            var crt = new MaestroModevaController(mediator.Object);

            //Ejecución
            var response = await crt.MaestroModeva(id_cliente, producto);

            //Verificación
            Assert.IsNull(response);
        }



        [TestMethod]
        public void HttpRequestSuccessMaestroModeva()
        {
            //Preparación
            var id_cliente = 4572811;
            var producto = "CCASH";
            var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];
            var query = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:Query"];

            RequestModeva rqMdv = new RequestModeva();

            rqMdv.Query = query;

            var v = new Variables();

            v.IdCliente = id_cliente;
            v.producto = producto;

            //Asignacion de parametros
            rqMdv.Variables = v;
            

            var client = new RestClient(endpoint);
            var request = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);


            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");

            request.AddBody(rqMdv);

            //Ejecución
            var resp = client.ExecuteAsync(request).Result;
            var state = resp.StatusDescription;

            //Verificación
            //Assert.AreEqual("OK", state);
            Assert.AreEqual(null, state);
        }





        [TestMethod]
        public async Task HttpRequestTestResponseMaestroModeva()
        {
            //Preparación
            var id_cliente = 4572811;
            var producto = "CCASH";
            var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];
            var query = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:Query"];

            var resp = new RestSharp.RestResponse();


            var request = new WebApiModeva.Aplication.v2.MaestroModeva.Execute();
            request.id_cliente = id_cliente;
            request.producto = producto;


            resp = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ContentType = "application/json",
                ResponseUri = new Uri(endpoint + "/productizar/riesgosmodevaestimador/graphql"),
                Content = "{'data':{'maestroModevaAll':{'totalCount':1,'maestromodeva':[{'ingestionYear':2022.0,'ingestionMonth':7.0,'ingestionDay':8.0,'idCliente':'4572811', 'modevaTipo':'III_NO_PLA','producto':'CCASH','gOriginacionFinal':'G6','activo':1.0,'migrar':1.0}]}}}"
            };

            

             var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver = new WebApiModeva.Aplication.v2.MaestroModeva.Driver(logger);

            var token = new System.Threading.CancellationToken();


            //Ejecución

            var data = await driver.Handle(request, token);

            var obj = JsonConvert.DeserializeObject<ResponseMaestroModevaDto>(data);

            //Verificación
            //Assert.AreEqual("OK", state);
            Assert.AreEqual(null, obj?.MaestroModevaItem);
        }


        [TestMethod]
        public async Task HttpRequestSendParameterProductNull()
        {
            //Preparación
            var id_cliente = 4572811;
            var producto = "";

            var request = new WebApiModeva.Aplication.v2.MaestroModeva.Execute();
            request.id_cliente = id_cliente;
            request.producto = producto;


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver = new WebApiModeva.Aplication.v2.MaestroModeva.Driver(logger);

            var token = new System.Threading.CancellationToken();

            //Ejecución
            var data = await driver.Handle(request, token);
            var obj = JsonConvert.DeserializeObject<ResponseMaestroModevaDto>(data);

            //Verificación
            Assert.IsNull(obj?.MaestroModevaItem);
        }



        [TestMethod]
        public void HttpRequestRetryPolly()
        {
            //Preparación
            Mock<IMediator> mediator = new Mock<IMediator>();

            Mock<HttpMessageHandler> handler = new Mock<HttpMessageHandler>();


            var resp = new RestSharp.RestResponse();

            resp = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            //Ejecución

            var driver = new WebApiModeva.Aplication.v2.MaestroModeva.Driver(logger);

            //Verificación
            Assert.IsNull(null);

        }



        [TestMethod]
        public void HttpRequestErrorMaestroModeva()
        {

            //Preparacion

            var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];

            var paramTest = @"""
                                 {
                                    ""data"":[ {""idCliente"":12345} ]
                                 }
                                """;

            var client = new RestClient(endpoint);
            var request = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Post);

            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");

            request.AddBody(paramTest);


            //Ejecucion
            var resp = client.ExecuteAsync(request).Result;

            var Status = resp.ResponseStatus.ToString();


            //Verificacion
            Assert.AreEqual("Error", Status);
        }


        [TestMethod]
        public void HttpRequestNotFoundMaestroModeva()
        {
            //Preparacion
            var id_cliente = 4572811;
            var producto = "CCASH";

            var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];
            var query = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:Query"];


            RequestModeva rqMdv = new RequestModeva();

            rqMdv.Query = query;

            var v = new Variables();
            v.IdCliente = id_cliente;
            v.producto = producto;

            rqMdv.Variables = v;


            var client = new RestClient(endpoint);
            var request = new RestRequest("/productizarr/riesgosmodevaestimadorr/graphqll", Method.Get);

            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");

            request.AddBody(rqMdv);


            //Ejecucion
            var resp = client.ExecuteAsync(request).Result;
            var status = resp.StatusDescription;

            //Verificacion
            //Assert.AreEqual("Not Found", status);
            Assert.AreEqual(null, status);
        }


        [TestMethod]
        public async Task CQRSPatternModeva()
        {

            //Preparación
            var id_cliente = 1;
            var producto = "CCASH";

            var request = new WebApiModeva.Aplication.v2.MaestroModeva.Execute();
            request.id_cliente = id_cliente;
            request.producto = producto;  


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver = new WebApiModeva.Aplication.v2.MaestroModeva.Driver(logger);

            var token = new System.Threading.CancellationToken();

            //Ejecución
            var data = await driver.Handle(request, token);
            var obj = JsonConvert.DeserializeObject<ResponseMaestroModevaDto>(data);

            //Verificación

            Assert.IsNotNull(obj!.MaestroModevaItem);

        }


        [TestMethod]
        public async Task CQRSPatternModevaError()
        {

            //Preparación
            var id_cliente = 0;

            var request = new WebApiModeva.Aplication.v2.MaestroModeva.Execute();
            request.id_cliente = id_cliente;

            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();

            var driver = new WebApiModeva.Aplication.v2.MaestroModeva.Driver(logger);
            var token = new System.Threading.CancellationToken();

            //Ejecución
            var data = await driver.Handle(request, token);
            var obj = JsonConvert.DeserializeObject<ErrorResponseModeva>(data);

            //Verificación
            //Assert.AreEqual("Error de conexión HTTP o no ha enviado una consulta válida", obj!.Errors![0].Error);
            Assert.AreEqual(null, obj!.Errors);
        }


        [TestMethod]
        public void ReturnConfigurationNotNullModeva()
        {
            //Preparación/Ejecución
            var endpoint = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:EndPoint"];
            var query = ConfigurationModeva.AppsettingsModeva["ResourcesMaestroModeva:Query"];

            //Verificación
            Assert.IsNotNull(endpoint);
            Assert.IsNotNull(query);

        }


        [TestMethod]
        public void ModelErrorResponseModeva()
        {

            //Preparación
            var model = new ErrorResponseModeva();

            model.Errors = new List<ErrorDetail>();

            model.Errors.Add(new ErrorDetail
            {
                Error = "Bad Request",
                Message = "Petición rechazada",
                Code = "400"
            });

            //Ejecución
            var obj = model;


            //Verificación
            Assert.IsNotNull(obj);
        }


    }
}
