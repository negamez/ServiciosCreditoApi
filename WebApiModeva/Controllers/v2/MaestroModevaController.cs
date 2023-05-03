using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiModeva.Aplication.v2;
using WebApiModeva.Dto;
using WebApiModeva.Model;

namespace WebApiModeva.Controllers.v2
{
    [ApiController]
    [Route("api/servicios-credito/grupos-modeva")]
    [ApiVersion("2.0")]
    public class MaestroModevaController : ControllerBase
    {
        private readonly IMediator Imediator;
        public MaestroModevaController(IMediator mediator)
        {
            Imediator = mediator;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<ResponseMaestroModeva>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ReturnDefaultError), StatusCodes.Status404NotFound)]
        public async Task<string> MaestroModeva([FromQuery] int id_cliente, string? producto)
        {
            return await Imediator.Send(new MaestroModeva.Execute { id_cliente = id_cliente, producto = producto });
        }

    }
}
