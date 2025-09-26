using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Interfaces;
using System.Security.Claims;

namespace InventorySystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de compras
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;
        private readonly ILogger<ComprasController> _logger;

        public ComprasController(ICompraService compraService, ILogger<ComprasController> logger)
        {
            _compraService = compraService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las compras
        /// </summary>
        /// <returns>Lista de compras</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompraDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var compras = await _compraService.GetAllAsync();
                return Ok(compras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener compras");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener compra por ID
        /// </summary>
        /// <param name="id">ID de la compra</param>
        /// <returns>Compra encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var compra = await _compraService.GetByIdAsync(id);
                if (compra == null)
                {
                    return NotFound(new { message = "Compra no encontrada" });
                }

                return Ok(compra);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener compra {CompraId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear nueva compra
        /// </summary>
        /// <param name="compraDto">Datos de la compra</param>
        /// <returns>Compra creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CompraDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CompraDto compraDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var compra = await _compraService.CreateAsync(compraDto, userId);
                _logger.LogInformation("Compra creada: {CompraId} por usuario {UserId}", compra.Id, userId);

                return CreatedAtAction(nameof(GetById), new { id = compra.Id }, compra);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear compra");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de compras por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Reporte de compras</returns>
        [HttpGet("reporte")]
        [ProducesResponseType(typeof(IEnumerable<ComprasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReporte([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                // Validar fechas
                if (fechaInicio > fechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });
                }

                var reporte = await _compraService.GetComprasReportAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de compras");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    /// <summary>
    /// Controlador para la gestión de ventas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly ILogger<VentasController> _logger;

        public VentasController(IVentaService ventaService, ILogger<VentasController> logger)
        {
            _ventaService = ventaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las ventas
        /// </summary>
        /// <returns>Lista de ventas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VentaDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ventas = await _ventaService.GetAllAsync();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener venta por ID
        /// </summary>
        /// <param name="id">ID de la venta</param>
        /// <returns>Venta encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var venta = await _ventaService.GetByIdAsync(id);
                if (venta == null)
                {
                    return NotFound(new { message = "Venta no encontrada" });
                }

                return Ok(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener venta {VentaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear nueva venta
        /// </summary>
        /// <param name="ventaDto">Datos de la venta</param>
        /// <returns>Venta creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VentaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] VentaDto ventaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var venta = await _ventaService.CreateAsync(ventaDto, userId);
                _logger.LogInformation("Venta creada: {VentaId} por usuario {UserId}", venta.Id, userId);

                return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear venta");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de ventas con filtros
        /// </summary>
        /// <param name="filter">Filtros para el reporte</param>
        /// <returns>Reporte de ventas</returns>
        [HttpPost("reporte")]
        [ProducesResponseType(typeof(IEnumerable<VentasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReporte([FromBody] VentasReportFilterDto filter)
        {
            try
            {
                // Validar fechas
                if (filter.FechaInicio > filter.FechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });
                }

                var reporte = await _ventaService.GetVentasReportAsync(filter);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de ventas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de ventas por rango de fechas (método GET para compatibilidad)
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="clienteId">ID del cliente (opcional)</param>
        /// <returns>Reporte de ventas</returns>
        [HttpGet("reporte")]
        [ProducesResponseType(typeof(IEnumerable<VentasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReporteQuery(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin, 
            [FromQuery] int? clienteId = null)
        {
            try
            {
                // Validar fechas
                if (fechaInicio > fechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });
                }

                var filter = new VentasReportFilterDto
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    ClienteId = clienteId
                };

                var reporte = await _ventaService.GetVentasReportAsync(filter);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de ventas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}