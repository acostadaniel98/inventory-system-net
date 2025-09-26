using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Core.DTOs
{
    /// <summary>
    /// DTO para crear/editar productos
    /// </summary>
    public class ProductoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para crear/editar proveedores
    /// </summary>
    public class ProveedorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string? Telefono { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para crear/editar clientes
    /// </summary>
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string? Telefono { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para los detalles de compra
    /// </summary>
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

    /// <summary>
    /// DTO para crear compras completas
    /// </summary>
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

    /// <summary>
    /// DTO para los detalles de venta
    /// </summary>
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

    /// <summary>
    /// DTO para crear ventas completas
    /// </summary>
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
}