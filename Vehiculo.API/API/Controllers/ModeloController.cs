using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeloController : ControllerBase, IModeloController
    {
        private IModeloFlujo _modeloFlujo;
        private ILogger<VehiculoController> _logger;

        public ModeloController(IModeloFlujo modeloFlujo, ILogger<VehiculoController> logger)
        {
            _modeloFlujo = modeloFlujo;
            _logger = logger;
        }

        #region Operaciones

        [HttpGet("{IdMarca}")]
        public async Task<IActionResult> Obtener([FromRoute] Guid IdMarca)
        {
            var resultado = await _modeloFlujo.Obtener(IdMarca);
            return Ok(resultado);
        }

        #endregion       
    }
}