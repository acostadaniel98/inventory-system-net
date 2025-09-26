using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Entities;
using InventorySystem.Core.Interfaces;
using InventorySystem.Infrastructure.Data;

namespace InventorySystem.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la gestión de compras
    /// </summary>
    public class CompraService : ICompraService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductoService _productoService;

        public CompraService(ApplicationDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        public async Task<IEnumerable<CompraDto>> GetAllAsync()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.CompraDetalles)
                    .ThenInclude(cd => cd.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return compras.Select(c => new CompraDto
            {
                Id = c.Id,
                ProveedorId = c.ProveedorId,
                ProveedorNombre = c.Proveedor.Nombre,
                UsuarioId = c.UsuarioId,
                UsuarioNombre = $"{c.Usuario.FirstName} {c.Usuario.LastName}",
                Fecha = c.Fecha,
                Total = c.Total,
                CompraDetalles = c.CompraDetalles.Select(cd => new CompraDetalleDto
                {
                    Id = cd.Id,
                    ProductoId = cd.ProductoId,
                    ProductoNombre = cd.Producto.Nombre,
                    Cantidad = cd.Cantidad,
                    PrecioCompra = cd.PrecioCompra
                }).ToList()
            });
        }

        public async Task<CompraDto?> GetByIdAsync(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.CompraDetalles)
                    .ThenInclude(cd => cd.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return null;

            return new CompraDto
            {
                Id = compra.Id,
                ProveedorId = compra.ProveedorId,
                ProveedorNombre = compra.Proveedor.Nombre,
                UsuarioId = compra.UsuarioId,
                UsuarioNombre = $"{compra.Usuario.FirstName} {compra.Usuario.LastName}",
                Fecha = compra.Fecha,
                Total = compra.Total,
                CompraDetalles = compra.CompraDetalles.Select(cd => new CompraDetalleDto
                {
                    Id = cd.Id,
                    ProductoId = cd.ProductoId,
                    ProductoNombre = cd.Producto.Nombre,
                    Cantidad = cd.Cantidad,
                    PrecioCompra = cd.PrecioCompra
                }).ToList()
            };
        }

        public async Task<CompraDto> CreateAsync(CompraDto compraDto, string usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear la compra
                var compra = new Compra
                {
                    ProveedorId = compraDto.ProveedorId,
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Total = 0 // Se calculará con los detalles
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Crear los detalles y actualizar stock
                decimal total = 0;
                var detalles = new List<CompraDetalle>();

                foreach (var detalleDto in compraDto.CompraDetalles)
                {
                    var detalle = new CompraDetalle
                    {
                        CompraId = compra.Id,
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        PrecioCompra = detalleDto.PrecioCompra
                    };

                    detalles.Add(detalle);
                    total += detalle.Subtotal;

                    // IMPORTANTE: Actualizar stock del producto (incrementar en compras)
                    await _productoService.UpdateStockAsync(detalleDto.ProductoId, detalleDto.Cantidad, true);
                }

                _context.CompraDetalles.AddRange(detalles);
                compra.Total = total;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Cargar los datos para la respuesta
                var compraCompleta = await GetByIdAsync(compra.Id);
                return compraCompleta!;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
    }

    /// <summary>
    /// Servicio para la gestión de ventas
    /// </summary>
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductoService _productoService;

        public VentaService(ApplicationDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        public async Task<IEnumerable<VentaDto>> GetAllAsync()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.VentaDetalles)
                    .ThenInclude(vd => vd.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return ventas.Select(v => new VentaDto
            {
                Id = v.Id,
                ClienteId = v.ClienteId,
                ClienteNombre = v.Cliente.Nombre,
                UsuarioId = v.UsuarioId,
                UsuarioNombre = $"{v.Usuario.FirstName} {v.Usuario.LastName}",
                Fecha = v.Fecha,
                Total = v.Total,
                VentaDetalles = v.VentaDetalles.Select(vd => new VentaDetalleDto
                {
                    Id = vd.Id,
                    ProductoId = vd.ProductoId,
                    ProductoNombre = vd.Producto.Nombre,
                    Cantidad = vd.Cantidad,
                    PrecioVenta = vd.PrecioVenta
                }).ToList()
            });
        }

        public async Task<VentaDto?> GetByIdAsync(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.VentaDetalles)
                    .ThenInclude(vd => vd.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return null;

            return new VentaDto
            {
                Id = venta.Id,
                ClienteId = venta.ClienteId,
                ClienteNombre = venta.Cliente.Nombre,
                UsuarioId = venta.UsuarioId,
                UsuarioNombre = $"{venta.Usuario.FirstName} {venta.Usuario.LastName}",
                Fecha = venta.Fecha,
                Total = venta.Total,
                VentaDetalles = venta.VentaDetalles.Select(vd => new VentaDetalleDto
                {
                    Id = vd.Id,
                    ProductoId = vd.ProductoId,
                    ProductoNombre = vd.Producto.Nombre,
                    Cantidad = vd.Cantidad,
                    PrecioVenta = vd.PrecioVenta
                }).ToList()
            };
        }

        public async Task<VentaDto> CreateAsync(VentaDto ventaDto, string usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear la venta
                var venta = new Venta
                {
                    ClienteId = ventaDto.ClienteId,
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Total = 0 // Se calculará con los detalles
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Crear los detalles y actualizar stock
                decimal total = 0;
                var detalles = new List<VentaDetalle>();

                foreach (var detalleDto in ventaDto.VentaDetalles)
                {
                    var detalle = new VentaDetalle
                    {
                        VentaId = venta.Id,
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        PrecioVenta = detalleDto.PrecioVenta
                    };

                    detalles.Add(detalle);
                    total += detalle.Subtotal;

                    // IMPORTANTE: Actualizar stock del producto (decrementar en ventas)
                    await _productoService.UpdateStockAsync(detalleDto.ProductoId, detalleDto.Cantidad, false);
                }

                _context.VentaDetalles.AddRange(detalles);
                venta.Total = total;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Cargar los datos para la respuesta
                var ventaCompleta = await GetByIdAsync(venta.Id);
                return ventaCompleta!;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
    }
}