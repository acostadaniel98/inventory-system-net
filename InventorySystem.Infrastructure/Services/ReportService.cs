using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Interfaces;
using InventorySystem.Infrastructure.Data;

namespace InventorySystem.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la generación de reportes
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockReportDto>> GetStockReportAsync()
        {
            var productos = await _context.Productos.ToListAsync();

            return productos.Select(p => new StockReportDto
            {
                ProductoId = p.Id,
                ProductoNombre = p.Nombre,
                ProductoDescripcion = p.Descripcion,
                PrecioUnitario = p.PrecioUnitario,
                Stock = p.Stock,
                EstadoStock = GetEstadoStock(p.Stock)
            }).OrderBy(p => p.Stock);
        }

        public async Task<IEnumerable<VentasReportDto>> GetVentasReportAsync(VentasReportFilterDto filter)
        {
            var query = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.VentaDetalles)
                    .ThenInclude(vd => vd.Producto)
                .Where(v => v.Fecha.Date >= filter.FechaInicio.Date && v.Fecha.Date <= filter.FechaFin.Date);

            if (filter.ClienteId.HasValue)
            {
                query = query.Where(v => v.ClienteId == filter.ClienteId.Value);
            }

            if (!string.IsNullOrEmpty(filter.UsuarioId))
            {
                query = query.Where(v => v.UsuarioId == filter.UsuarioId);
            }

            var ventas = await query.OrderByDescending(v => v.Fecha).ToListAsync();

            return ventas.Select(v => new VentasReportDto
            {
                VentaId = v.Id,
                FechaVenta = v.Fecha,
                ClienteNombre = v.Cliente.Nombre,
                VendedorNombre = $"{v.Usuario.FirstName} {v.Usuario.LastName}",
                Total = v.Total,
                CantidadProductos = v.VentaDetalles.Sum(vd => vd.Cantidad),
                Detalles = v.VentaDetalles.Select(vd => new VentaDetalleReportDto
                {
                    ProductoNombre = vd.Producto.Nombre,
                    Cantidad = vd.Cantidad,
                    PrecioVenta = vd.PrecioVenta,
                    Subtotal = vd.Subtotal
                }).ToList()
            });
        }

        public async Task<IEnumerable<ComprasReportDto>> GetComprasReportAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.CompraDetalles)
                    .ThenInclude(cd => cd.Producto)
                .Where(c => c.Fecha.Date >= fechaInicio.Date && c.Fecha.Date <= fechaFin.Date)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return compras.Select(c => new ComprasReportDto
            {
                CompraId = c.Id,
                FechaCompra = c.Fecha,
                ProveedorNombre = c.Proveedor.Nombre,
                UsuarioNombre = $"{c.Usuario.FirstName} {c.Usuario.LastName}",
                Total = c.Total,
                CantidadProductos = c.CompraDetalles.Sum(cd => cd.Cantidad),
                Detalles = c.CompraDetalles.Select(cd => new CompraDetalleReportDto
                {
                    ProductoNombre = cd.Producto.Nombre,
                    Cantidad = cd.Cantidad,
                    PrecioCompra = cd.PrecioCompra,
                    Subtotal = cd.Subtotal
                }).ToList()
            });
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var fechaInicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var fechaFinMes = fechaInicioMes.AddMonths(1).AddDays(-1);

            var stats = new DashboardStatsDto
            {
                TotalProductos = await _context.Productos.CountAsync(),
                ProductosBajoStock = await _context.Productos.CountAsync(p => p.Stock <= 10), // Consideramos bajo stock <= 10
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalProveedores = await _context.Proveedores.CountAsync()
            };

            // Estadísticas del mes actual
            var ventasDelMes = await _context.Ventas
                .Where(v => v.Fecha >= fechaInicioMes && v.Fecha <= fechaFinMes)
                .ToListAsync();

            var comprasDelMes = await _context.Compras
                .Where(c => c.Fecha >= fechaInicioMes && c.Fecha <= fechaFinMes)
                .ToListAsync();

            stats.VentasDelMes = ventasDelMes.Sum(v => v.Total);
            stats.VentasCountDelMes = ventasDelMes.Count;
            stats.ComprasDelMes = comprasDelMes.Sum(c => c.Total);
            stats.ComprasCountDelMes = comprasDelMes.Count;

            return stats;
        }

        private static string GetEstadoStock(int stock)
        {
            return stock switch
            {
                <= 5 => "Crítico",
                <= 10 => "Bajo",
                <= 50 => "Normal",
                _ => "Alto"
            };
        }
    }
}