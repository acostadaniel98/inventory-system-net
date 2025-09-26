using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Entities;
using InventorySystem.Core.Interfaces;
using InventorySystem.Infrastructure.Data;

namespace InventorySystem.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la gestión de productos
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IRepository<Producto> _repository;
        private readonly ApplicationDbContext _context;

        public ProductoService(IRepository<Producto> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync()
        {
            var productos = await _repository.GetAllAsync();
            return productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                PrecioUnitario = p.PrecioUnitario,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null) return null;

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioUnitario = producto.PrecioUnitario,
                Stock = producto.Stock,
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt
            };
        }

        public async Task<ProductoDto> CreateAsync(ProductoDto productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                PrecioUnitario = productoDto.PrecioUnitario,
                Stock = productoDto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            var createdProducto = await _repository.CreateAsync(producto);

            return new ProductoDto
            {
                Id = createdProducto.Id,
                Nombre = createdProducto.Nombre,
                Descripcion = createdProducto.Descripcion,
                PrecioUnitario = createdProducto.PrecioUnitario,
                Stock = createdProducto.Stock,
                CreatedAt = createdProducto.CreatedAt,
                UpdatedAt = createdProducto.UpdatedAt
            };
        }

        public async Task<ProductoDto> UpdateAsync(int id, ProductoDto productoDto)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null)
                throw new ArgumentException("Producto no encontrado");

            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.PrecioUnitario = productoDto.PrecioUnitario;
            producto.Stock = productoDto.Stock;
            producto.UpdatedAt = DateTime.UtcNow;

            var updatedProducto = await _repository.UpdateAsync(producto);

            return new ProductoDto
            {
                Id = updatedProducto.Id,
                Nombre = updatedProducto.Nombre,
                Descripcion = updatedProducto.Descripcion,
                PrecioUnitario = updatedProducto.PrecioUnitario,
                Stock = updatedProducto.Stock,
                CreatedAt = updatedProducto.CreatedAt,
                UpdatedAt = updatedProducto.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Verificar si el producto tiene movimientos
            var tieneCompras = await _context.CompraDetalles.AnyAsync(cd => cd.ProductoId == id);
            var tieneVentas = await _context.VentaDetalles.AnyAsync(vd => vd.ProductoId == id);

            if (tieneCompras || tieneVentas)
            {
                throw new InvalidOperationException("No se puede eliminar el producto porque tiene movimientos registrados");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> UpdateStockAsync(int productoId, int cantidad, bool esCompra = true)
        {
            var producto = await _repository.GetByIdAsync(productoId);
            if (producto == null)
                return false;

            if (esCompra)
            {
                // Compra: incrementar stock
                producto.Stock += cantidad;
            }
            else
            {
                // Venta: decrementar stock
                if (producto.Stock < cantidad)
                {
                    throw new InvalidOperationException($"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad solicitada: {cantidad}");
                }
                producto.Stock -= cantidad;
            }

            producto.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(producto);
            return true;
        }
    }

    /// <summary>
    /// Servicio para la gestión de proveedores
    /// </summary>
    public class ProveedorService : IProveedorService
    {
        private readonly IRepository<Proveedor> _repository;
        private readonly ApplicationDbContext _context;

        public ProveedorService(IRepository<Proveedor> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ProveedorDto>> GetAllAsync()
        {
            var proveedores = await _repository.GetAllAsync();
            return proveedores.Select(p => new ProveedorDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Email = p.Email,
                Telefono = p.Telefono,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<ProveedorDto?> GetByIdAsync(int id)
        {
            var proveedor = await _repository.GetByIdAsync(id);
            if (proveedor == null) return null;

            return new ProveedorDto
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono,
                CreatedAt = proveedor.CreatedAt
            };
        }

        public async Task<ProveedorDto> CreateAsync(ProveedorDto proveedorDto)
        {
            // Verificar si el email ya existe
            var existeEmail = await _context.Proveedores.AnyAsync(p => p.Email == proveedorDto.Email);
            if (existeEmail)
                throw new InvalidOperationException("Ya existe un proveedor con este email");

            var proveedor = new Proveedor
            {
                Nombre = proveedorDto.Nombre,
                Email = proveedorDto.Email,
                Telefono = proveedorDto.Telefono,
                CreatedAt = DateTime.UtcNow
            };

            var createdProveedor = await _repository.CreateAsync(proveedor);

            return new ProveedorDto
            {
                Id = createdProveedor.Id,
                Nombre = createdProveedor.Nombre,
                Email = createdProveedor.Email,
                Telefono = createdProveedor.Telefono,
                CreatedAt = createdProveedor.CreatedAt
            };
        }

        public async Task<ProveedorDto> UpdateAsync(int id, ProveedorDto proveedorDto)
        {
            var proveedor = await _repository.GetByIdAsync(id);
            if (proveedor == null)
                throw new ArgumentException("Proveedor no encontrado");

            // Verificar si el email ya existe en otro proveedor
            var existeEmail = await _context.Proveedores.AnyAsync(p => p.Email == proveedorDto.Email && p.Id != id);
            if (existeEmail)
                throw new InvalidOperationException("Ya existe un proveedor con este email");

            proveedor.Nombre = proveedorDto.Nombre;
            proveedor.Email = proveedorDto.Email;
            proveedor.Telefono = proveedorDto.Telefono;

            var updatedProveedor = await _repository.UpdateAsync(proveedor);

            return new ProveedorDto
            {
                Id = updatedProveedor.Id,
                Nombre = updatedProveedor.Nombre,
                Email = updatedProveedor.Email,
                Telefono = updatedProveedor.Telefono,
                CreatedAt = updatedProveedor.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Verificar si el proveedor tiene compras
            var tieneCompras = await _context.Compras.AnyAsync(c => c.ProveedorId == id);
            if (tieneCompras)
            {
                throw new InvalidOperationException("No se puede eliminar el proveedor porque tiene compras registradas");
            }

            return await _repository.DeleteAsync(id);
        }
    }

    /// <summary>
    /// Servicio para la gestión de clientes
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly IRepository<Cliente> _repository;
        private readonly ApplicationDbContext _context;

        public ClienteService(IRepository<Cliente> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            var clientes = await _repository.GetAllAsync();
            return clientes.Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                Telefono = c.Telefono,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                CreatedAt = cliente.CreatedAt
            };
        }

        public async Task<ClienteDto> CreateAsync(ClienteDto clienteDto)
        {
            // Verificar si el email ya existe
            var existeEmail = await _context.Clientes.AnyAsync(c => c.Email == clienteDto.Email);
            if (existeEmail)
                throw new InvalidOperationException("Ya existe un cliente con este email");

            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Email = clienteDto.Email,
                Telefono = clienteDto.Telefono,
                CreatedAt = DateTime.UtcNow
            };

            var createdCliente = await _repository.CreateAsync(cliente);

            return new ClienteDto
            {
                Id = createdCliente.Id,
                Nombre = createdCliente.Nombre,
                Email = createdCliente.Email,
                Telefono = createdCliente.Telefono,
                CreatedAt = createdCliente.CreatedAt
            };
        }

        public async Task<ClienteDto> UpdateAsync(int id, ClienteDto clienteDto)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado");

            // Verificar si el email ya existe en otro cliente
            var existeEmail = await _context.Clientes.AnyAsync(c => c.Email == clienteDto.Email && c.Id != id);
            if (existeEmail)
                throw new InvalidOperationException("Ya existe un cliente con este email");

            cliente.Nombre = clienteDto.Nombre;
            cliente.Email = clienteDto.Email;
            cliente.Telefono = clienteDto.Telefono;

            var updatedCliente = await _repository.UpdateAsync(cliente);

            return new ClienteDto
            {
                Id = updatedCliente.Id,
                Nombre = updatedCliente.Nombre,
                Email = updatedCliente.Email,
                Telefono = updatedCliente.Telefono,
                CreatedAt = updatedCliente.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Verificar si el cliente tiene ventas
            var tieneVentas = await _context.Ventas.AnyAsync(v => v.ClienteId == id);
            if (tieneVentas)
            {
                throw new InvalidOperationException("No se puede eliminar el cliente porque tiene ventas registradas");
            }

            return await _repository.DeleteAsync(id);
        }
    }
}