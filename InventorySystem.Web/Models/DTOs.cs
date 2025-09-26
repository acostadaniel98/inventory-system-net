using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Web.Models
{
    /// <summary>
    /// DTOs compartidos para el cliente web
    /// </summary>
    
    // DTOs de Autenticación
    public class LoginDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido")]
        public string Role { get; set; } = "Vendedor";
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime ExpireDate { get; set; }
    }

    // DTOs de Negocio
    public class ProductoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [MaxLength(50, ErrorMessage = "El código no puede tener más de 50 caracteres")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El stock mínimo es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int StockMinimo { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }

    public class ProveedorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El RUC no puede tener más de 20 caracteres")]
        public string Ruc { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "El contacto no puede tener más de 100 caracteres")]
        public string Contacto { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }

    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }

    public class CompraDetalleDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        [Required(ErrorMessage = "El precio de compra es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioCompra { get; set; }
        
        public decimal Subtotal => Cantidad * PrecioCompra;
    }

    public class CompraDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El proveedor es requerido")]
        public int ProveedorId { get; set; }
        
        public string ProveedorNombre { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un producto")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<CompraDetalleDto> CompraDetalles { get; set; } = new List<CompraDetalleDto>();
    }

    public class VentaDetalleDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        [Required(ErrorMessage = "El precio de venta es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioVenta { get; set; }
        
        public decimal Subtotal => Cantidad * PrecioVenta;
    }

    public class VentaDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ClienteId { get; set; }
        
        public string ClienteNombre { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un producto")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<VentaDetalleDto> VentaDetalles { get; set; } = new List<VentaDetalleDto>();
    }

    // DTOs de Reportes
    public class StockReportDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? ProductoDescripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }
        public string EstadoStock { get; set; } = string.Empty;
    }

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

    public class VentaDetalleReportDto
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class VentasReportFilterDto
    {
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow.Date.AddDays(-30);
        public DateTime FechaFin { get; set; } = DateTime.UtcNow.Date;
        public int? ClienteId { get; set; }
        public string? UsuarioId { get; set; }
    }

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

    public class CompraDetalleReportDto
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Subtotal { get; set; }
    }

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

    // Clases auxiliares
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class UserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsAuthenticated { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool IsAdmin => Roles.Contains("Admin");
        public bool IsVendedor => Roles.Contains("Vendedor");
    }
}