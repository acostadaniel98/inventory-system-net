namespace InventorySystem.Core.DTOs
{
    /// <summary>
    /// DTO para reportes de stock de productos
    /// </summary>
    public class StockReportDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? ProductoDescripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }
        public string EstadoStock { get; set; } = string.Empty; // Bajo, Normal, Alto
    }

    /// <summary>
    /// DTO para reportes de ventas
    /// </summary>
    public class VentasReportDto
    {
        public DateTime FechaVenta { get; set; }
        public int VentaId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string VendedorNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int CantidadProductos { get; set; }
        public List<VentaDetalleReportDto> Detalles { get; set; } = new List<VentaDetalleReportDto>();
    }

    /// <summary>
    /// DTO para los detalles en reportes de ventas
    /// </summary>
    public class VentaDetalleReportDto
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// DTO para filtros de reportes de ventas
    /// </summary>
    public class VentasReportFilterDto
    {
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow.Date.AddDays(-30);
        public DateTime FechaFin { get; set; } = DateTime.UtcNow.Date;
        public int? ClienteId { get; set; }
        public string? UsuarioId { get; set; }
    }

    /// <summary>
    /// DTO para reportes de compras
    /// </summary>
    public class ComprasReportDto
    {
        public DateTime FechaCompra { get; set; }
        public int CompraId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int CantidadProductos { get; set; }
        public List<CompraDetalleReportDto> Detalles { get; set; } = new List<CompraDetalleReportDto>();
    }

    /// <summary>
    /// DTO para los detalles en reportes de compras
    /// </summary>
    public class CompraDetalleReportDto
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// DTO para resumen de estadísticas del dashboard
    /// </summary>
    public class DashboardStatsDto
    {
        public int TotalProductos { get; set; }
        public int ProductosBajoStock { get; set; }
        public decimal VentasDelMes { get; set; }
        public decimal ComprasDelMes { get; set; }
        public int VentasCountDelMes { get; set; }
        public int ComprasCountDelMes { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProveedores { get; set; }
    }
}