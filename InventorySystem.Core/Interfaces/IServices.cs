using InventorySystem.Core.DTOs;

namespace InventorySystem.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de autenticación
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> LogoutAsync(string userId);
    }

    /// <summary>
    /// Interfaz genérica para repositorios
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }

    /// <summary>
    /// Interfaz para el servicio de productos
    /// </summary>
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync();
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto> CreateAsync(ProductoDto productoDto);
        Task<ProductoDto> UpdateAsync(int id, ProductoDto productoDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStockAsync(int productoId, int cantidad, bool esCompra = true);
    }

    /// <summary>
    /// Interfaz para el servicio de proveedores
    /// </summary>
    public interface IProveedorService
    {
        Task<IEnumerable<ProveedorDto>> GetAllAsync();
        Task<ProveedorDto?> GetByIdAsync(int id);
        Task<ProveedorDto> CreateAsync(ProveedorDto proveedorDto);
        Task<ProveedorDto> UpdateAsync(int id, ProveedorDto proveedorDto);
        Task<bool> DeleteAsync(int id);
    }

    /// <summary>
    /// Interfaz para el servicio de clientes
    /// </summary>
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteDto clienteDto);
        Task<ClienteDto> UpdateAsync(int id, ClienteDto clienteDto);
        Task<bool> DeleteAsync(int id);
    }

    /// <summary>
    /// Interfaz para el servicio de compras
    /// </summary>
    public interface ICompraService
    {
        Task<IEnumerable<CompraDto>> GetAllAsync();
        Task<CompraDto?> GetByIdAsync(int id);
        Task<CompraDto> CreateAsync(CompraDto compraDto, string usuarioId);
        Task<IEnumerable<ComprasReportDto>> GetComprasReportAsync(DateTime fechaInicio, DateTime fechaFin);
    }

    /// <summary>
    /// Interfaz para el servicio de ventas
    /// </summary>
    public interface IVentaService
    {
        Task<IEnumerable<VentaDto>> GetAllAsync();
        Task<VentaDto?> GetByIdAsync(int id);
        Task<VentaDto> CreateAsync(VentaDto ventaDto, string usuarioId);
        Task<IEnumerable<VentasReportDto>> GetVentasReportAsync(VentasReportFilterDto filter);
    }

    /// <summary>
    /// Interfaz para el servicio de reportes
    /// </summary>
    public interface IReportService
    {
        Task<IEnumerable<StockReportDto>> GetStockReportAsync();
        Task<IEnumerable<VentasReportDto>> GetVentasReportAsync(VentasReportFilterDto filter);
        Task<IEnumerable<ComprasReportDto>> GetComprasReportAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}