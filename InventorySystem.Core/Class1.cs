using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Core.Entities
{
    /// <summary>
    /// Usuario del sistema con Identity
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Relaciones
        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }

    /// <summary>
    /// Entidad Producto con control de stock
    /// </summary>
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Descripcion { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }
        
        public int Stock { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Relaciones
        public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
        public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }

    /// <summary>
    /// Entidad Proveedor
    /// </summary>
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Relaciones
        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }

    /// <summary>
    /// Entidad Cliente
    /// </summary>
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Relaciones
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }

    /// <summary>
    /// Entidad Compra (Header)
    /// </summary>
    public class Compra
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ProveedorId { get; set; }
        
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        // Relaciones
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; } = null!;
        
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; } = null!;
        
        public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
    }

    /// <summary>
    /// Entidad CompraDetalle (Detail)
    /// </summary>
    public class CompraDetalle
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CompraId { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        
        [Required]
        public int Cantidad { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCompra { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal => Cantidad * PrecioCompra;
        
        // Relaciones
        [ForeignKey("CompraId")]
        public virtual Compra Compra { get; set; } = null!;
        
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;
    }

    /// <summary>
    /// Entidad Venta (Header)
    /// </summary>
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        // Relaciones
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; } = null!;
        
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; } = null!;
        
        public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }

    /// <summary>
    /// Entidad VentaDetalle (Detail)
    /// </summary>
    public class VentaDetalle
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int VentaId { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        
        [Required]
        public int Cantidad { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal => Cantidad * PrecioVenta;
        
        // Relaciones
        [ForeignKey("VentaId")]
        public virtual Venta Venta { get; set; } = null!;
        
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;
    }
}
