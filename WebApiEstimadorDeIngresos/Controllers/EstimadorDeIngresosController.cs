using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebApiEstimadorDeIngresos.Aplication;
using WebApiEstimadorDeIngresos.Dto;
using WebApiEstimadorDeIngresos.Model;

namespace WebApiEstimadorDeIngresos.Controllers
{
    [ApiController]
    [Route("api/servicios-credito/estimador-ingresos")]
    [ApiVersion("1.0")]
    public class EstimadorDeIngresosController : ControllerBase
    {
        private readonly IMediator Imediator;

        public EstimadorDeIngresosController(IMediator mediator)
        {
            Imediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseEstimadorIngresos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ReturnDefaultError), StatusCodes.Status404NotFound)]
        public async Task<string> EstimadorDeIngresos([FromQuery] int id_cliente)
        {
            return await Imediator.Send(new EstimadorDeIngresos.Execute { id_cliente = id_cliente });
        }

    }
}
