using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Interfaces;

namespace InventorySystem.API.Controllers
{
    /// <summary>
    /// Controlador para la generación de reportes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReportService reportService, ILogger<ReportesController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener reporte de stock de productos
        /// </summary>
        /// <returns>Reporte de stock</returns>
        [HttpGet("stock")]
        [ProducesResponseType(typeof(IEnumerable<StockReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStockReport()
        {
            try
            {
                var reporte = await _reportService.GetStockReportAsync();
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de stock");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de ventas con filtros
        /// </summary>
        /// <param name="filter">Filtros para el reporte</param>
        /// <returns>Reporte de ventas</returns>
        [HttpPost("ventas")]
        [ProducesResponseType(typeof(IEnumerable<VentasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVentasReport([FromBody] VentasReportFilterDto filter)
        {
            try
            {
                // Validar fechas
                if (filter.FechaInicio > filter.FechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });
                }

                var reporte = await _reportService.GetVentasReportAsync(filter);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de ventas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de compras por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Reporte de compras</returns>
        [HttpGet("compras")]
        [ProducesResponseType(typeof(IEnumerable<ComprasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComprasReport([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                // Validar fechas
                if (fechaInicio > fechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });
                }

                var reporte = await _reportService.GetComprasReportAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de compras");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener estadísticas del dashboard
        /// </summary>
        /// <returns>Estadísticas principales del sistema</returns>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _reportService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener reporte de ventas por rango de fechas (método GET alternativo)
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="clienteId">ID del cliente (opcional)</param>
        /// <returns>Reporte de ventas</returns>
        [HttpGet("ventas")]
        [ProducesResponseType(typeof(IEnumerable<VentasReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVentasReportQuery(
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

                var reporte = await _reportService.GetVentasReportAsync(filter);
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