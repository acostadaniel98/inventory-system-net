using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.Entities;

namespace InventorySystem.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos principal del sistema
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades del dominio
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> CompraDetalles { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones de entidades
            ConfigureProducto(modelBuilder);
            ConfigureProveedor(modelBuilder);
            ConfigureCliente(modelBuilder);
            ConfigureCompra(modelBuilder);
            ConfigureCompraDetalle(modelBuilder);
            ConfigureVenta(modelBuilder);
            ConfigureVentaDetalle(modelBuilder);
            ConfigureApplicationUser(modelBuilder);

            // Configurar nombres de tablas de Identity en español
            modelBuilder.Entity<ApplicationUser>().ToTable("Usuarios");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UsuarioRoles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UsuarioTokens");
        }

        private static void ConfigureApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

        private static void ConfigureProducto(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.PrecioUnitario)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Stock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Nombre);
            });
        }

        private static void ConfigureProveedor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.Nombre);
            });
        }

        private static void ConfigureCliente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.Nombre);
            });
        }

        private static void ConfigureCompra(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compra>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Proveedor)
                    .WithMany(p => p.Compras)
                    .HasForeignKey(d => d.ProveedorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.Compras)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.ProveedorId);
            });
        }

        private static void ConfigureCompraDetalle(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompraDetalle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PrecioCompra)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Cantidad)
                    .IsRequired();

                entity.HasOne(d => d.Compra)
                    .WithMany(p => p.CompraDetalles)
                    .HasForeignKey(d => d.CompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.CompraDetalles)
                    .HasForeignKey(d => d.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CompraId);
                entity.HasIndex(e => e.ProductoId);
            });
        }

        private static void ConfigureVenta(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Ventas)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.Ventas)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.ClienteId);
            });
        }

        private static void ConfigureVentaDetalle(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VentaDetalle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PrecioVenta)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Cantidad)
                    .IsRequired();

                entity.HasOne(d => d.Venta)
                    .WithMany(p => p.VentaDetalles)
                    .HasForeignKey(d => d.VentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.VentaDetalles)
                    .HasForeignKey(d => d.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.VentaId);
                entity.HasIndex(e => e.ProductoId);
            });
        }
    }
}
