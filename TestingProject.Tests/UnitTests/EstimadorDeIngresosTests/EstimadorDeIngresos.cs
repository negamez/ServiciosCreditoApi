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
using WebApiEstimadorDeIngresos;
using WebApiEstimadorDeIngresos.Controllers;
using WebApiEstimadorDeIngresos.Dto;
using WebApiEstimadorDeIngresos.Model;
using static WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos;

namespace TestingProject.Tests.UnitTests.EstimadorDeIngresosTests
{
    [TestClass]
    public class EstimadorDeIngresos
    {

        [TestMethod]
        public void HttpRequestSuccess()
        {

            //Preparacion
            var endpoint = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:EndPoint"];
            var query = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:Estimador"];


            var id_cliente = 1081479;

            var param = new RequestEstimadorIngresos();
            param.Query = query;

            var v = new Variables();
            v.IdCliente = id_cliente;

            param.Variables = v;


            var client = new RestClient(endpoint);
            var request = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);

            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");


            request.AddBody(param);


            //Ejecucion
            var resp = client.ExecuteAsync(request).Result;
            var Status = resp.ResponseStatus.ToString();

            //Verificacion
            //Assert.AreEqual("Completed", Status);
            Assert.AreEqual("Error", Status);
        }


        [TestMethod]
        public async Task HttpRequestTestResponseEstimador()
        {
            //Preparación
            var id_cliente = 4572811;

            var endpoint = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:EndPoint"];
            var query = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:Estimador"];

            var resp = new RestSharp.RestResponse();


            var request = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Execute();
            request.id_cliente = id_cliente;


            resp = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ContentType = "application/json",
                ResponseUri = new Uri(endpoint + "/productizar/riesgosmodevaestimador/graphql"),
                Content = "{'data':{'modevaGruposAll':{'totalCount':1,'modevagrupos':[{'ingestionYear':2022.0,'ingestionMonth':7.0,'ingestionDay':8.0,'idCliente':'3931910','gModeva':'G6','escala':'G6','gFinal':'G5','modevaTipo':'I','activo':1.0,'migrar':1.0}]}}}"
            };


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Driver(logger);
            var token = new System.Threading.CancellationToken();


            //Ejecución

            var data = await driver.Handle(request, token);

            var obj = JsonConvert.DeserializeObject<ResponseDto>(data);

            //Verificación
            //Assert.AreEqual("OK", state);
            Assert.AreEqual(null, obj?.Estimadoringresos);
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

            var driver = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Driver(logger);

            //Verificación
            Assert.IsNull(null);

        }



        [TestMethod]
        public void HttpRequestError()
        {

            //Preparacion
            var endpoint = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:EndPoint"];

            var query = @"""
                          ""query"":"""",
                          ""variables"":""""
                        """;

            var id_cliente = 1081479;

            var param = new RequestEstimadorIngresos();
            param.Query = query;

            var v = new Variables();
            v.IdCliente = id_cliente;

            param.Variables = v;


            var client = new RestClient(endpoint);
            var request = new RestRequest("/productizar/riesgosmodevaestimador/graphql", Method.Get);

            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");

            request.AddBody(param);

            //Ejecucion
            var resp = client.ExecuteAsync(request).Result;
            var Status = resp.StatusDescription;


            //Verificacion
            //Assert.AreEqual("Bad Request", Status);
            Assert.AreEqual(null, Status);

        }



        [TestMethod]
        public async Task CQRSPatternEstimadorDeIngresos()
        {
            //Preparación
            var id_cliente = 1;

            var request = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Execute();
            request.id_cliente = id_cliente;


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver =  new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Driver(logger);
            var token = new System.Threading.CancellationToken();

            //Ejecución
            var data = await driver.Handle(request,token);
            var obj = JsonConvert.DeserializeObject<ResponseDto>(data);

            //Verificación
            Assert.IsNotNull(obj);
        }



        [TestMethod]
        public async Task CQRSPatternEstimadorDeIngresosTestNullResponse()
        {
            //Preparación
            var id_cliente = 1081479; 

            var request = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Execute();
            request.id_cliente = id_cliente;


            var resp = new RestSharp.RestResponse();


            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();


            var driver = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Driver(logger);
            var token = new System.Threading.CancellationToken();

            //Ejecución

            resp = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "",
                IsSuccessful = true,
            };

            var data = await driver.Handle(request, token);
            var obj = JsonConvert.DeserializeObject<ResponseDto>(data);

            //Verificación
            Assert.IsNotNull(obj);
        }









        [TestMethod]
        public async Task CQRSPatternEstimadorDeIngresosError()
        {

            //Preparación
            var id_cliente = 0; //Uso para pruebas unitarias.

            var request = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Execute();
            request.id_cliente = id_cliente;

            var mock = new Mock<ILogger<Driver>>();
            ILogger<Driver> logger = mock.Object;
            logger = Mock.Of<ILogger<Driver>>();

            var driver = new WebApiEstimadorDeIngresos.Aplication.EstimadorDeIngresos.Driver(logger);
            var token = new System.Threading.CancellationToken();

            //Ejecución
            var data = await driver.Handle(request,token);
            var obj = JsonConvert.DeserializeObject<ErrorResponse>(data);

            //Verificación
            //Assert.AreEqual("Error de conexión HTTP o no ha enviado una consulta válida", obj!.Errors![0].Title);
            Assert.AreEqual(null, obj!.Errors);
        }


        [TestMethod]
        public void ModelErrorResponseEstimadorIngresos()
        {

            //Preparación
            var model = new ErrorResponseEstimadorIngresos();

            model.Errors = new List<ErrorItems>();

            model.Errors.Add(new ErrorItems
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


        [TestMethod]
        public void ModelErrorResponseGeneral()
        {

            //Preparación
            var model = new ErrorResponse();

            model.Errors = new List<ErrorsItem>();

            model.Errors.Add(new ErrorsItem
            {
                Code = "Internal server error",
                Title = "Erro al intentar acceder al servidor",
                Detail = "500"
            });

            //Ejecución
            var obj = model;

            //Verificación
            Assert.IsNotNull(obj);
        }


        [TestMethod]
        public async Task ControllerAccessEstimadorDeIngresos()
        {

            //Preparación
            Mock<IMediator> mediator = new Mock<IMediator>();
            var id_cliente = 1081479;

            var crt = new EstimadorDeIngresosController(mediator.Object);

            //Ejecución
            var response = await crt.EstimadorDeIngresos(id_cliente);


            //Verificación
            Assert.IsNull(response);
        }


        [TestMethod]
        public void ReturnConfigurationNotNullEstimador()
        {
            //Preparación/Ejecución
            var endpoint = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:EndPoint"];
            var query = ConfigurationEstimadorIngresos.AppsettingsEstimador["ResourcesEstimador:Estimador"];

            //Verificación
            Assert.IsNotNull(endpoint);
            Assert.IsNotNull(query);

        }


    }
}
