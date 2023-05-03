using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiModeva.Aplication.v1;
using WebApiModeva.Dto;
using WebApiModeva.Model;

namespace WebApiModeva.Controllers.v1
{
    [ApiController]
    [Route("api/servicios-credito/grupos-modeva")]
    [ApiVersion("1.0")]
    public class GroupsModevaController : ControllerBase
    {
        private readonly IMediator Imediator;

        public GroupsModevaController(IMediator mediator)
        {
            Imediator = mediator;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(List<ResponseModeva>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ReturnDefaultError), StatusCodes.Status404NotFound)]
        public async Task<string> GroupsModeva([FromQuery] int id_cliente)
        {
            return await Imediator.Send(new GroupsModeva.Execute { id_cliente = id_cliente });
        }
    }
}
