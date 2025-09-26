using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Interfaces;
using System.Security.Claims;

namespace InventorySystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los productos
        /// </summary>
        /// <returns>Lista de productos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productos = await _productoService.GetAllAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {ProductoId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear nuevo producto
        /// </summary>
        /// <param name="productoDto">Datos del producto</param>
        /// <returns>Producto creado</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProductoDto productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var producto = await _productoService.CreateAsync(productoDto);
                _logger.LogInformation("Producto creado: {ProductoId} - {Nombre}", producto.Id, producto.Nombre);

                return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="productoDto">Datos actualizados del producto</param>
        /// <returns>Producto actualizado</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoDto productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var producto = await _productoService.UpdateAsync(id, productoDto);
                _logger.LogInformation("Producto actualizado: {ProductoId} - {Nombre}", producto.Id, producto.Nombre);

                return Ok(producto);
            }
            catch (ArgumentException)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {ProductoId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Eliminar producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productoService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                _logger.LogInformation("Producto eliminado: {ProductoId}", id);
                return Ok(new { message = "Producto eliminado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {ProductoId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    /// <summary>
    /// Controlador para la gestión de proveedores
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(IProveedorService proveedorService, ILogger<ProveedoresController> logger)
        {
            _proveedorService = proveedorService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProveedorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var proveedores = await _proveedorService.GetAllAsync();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var proveedor = await _proveedorService.GetByIdAsync(id);
                if (proveedor == null)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor {ProveedorId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProveedorDto proveedorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var proveedor = await _proveedorService.CreateAsync(proveedorDto);
                _logger.LogInformation("Proveedor creado: {ProveedorId} - {Nombre}", proveedor.Id, proveedor.Nombre);

                return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProveedorDto proveedorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var proveedor = await _proveedorService.UpdateAsync(id, proveedorDto);
                _logger.LogInformation("Proveedor actualizado: {ProveedorId} - {Nombre}", proveedor.Id, proveedor.Nombre);

                return Ok(proveedor);
            }
            catch (ArgumentException)
            {
                return NotFound(new { message = "Proveedor no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor {ProveedorId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _proveedorService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                _logger.LogInformation("Proveedor eliminado: {ProveedorId}", id);
                return Ok(new { message = "Proveedor eliminado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor {ProveedorId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    /// <summary>
    /// Controlador para la gestión de clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clientes = await _clienteService.GetAllAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cliente = await _clienteService.GetByIdAsync(id);
                if (cliente == null)
                {
                    return NotFound(new { message = "Cliente no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ClienteDto clienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cliente = await _clienteService.CreateAsync(clienteDto);
                _logger.LogInformation("Cliente creado: {ClienteId} - {Nombre}", cliente.Id, cliente.Nombre);

                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteDto clienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cliente = await _clienteService.UpdateAsync(id, clienteDto);
                _logger.LogInformation("Cliente actualizado: {ClienteId} - {Nombre}", cliente.Id, cliente.Nombre);

                return Ok(cliente);
            }
            catch (ArgumentException)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _clienteService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Cliente no encontrado" });
                }

                _logger.LogInformation("Cliente eliminado: {ClienteId}", id);
                return Ok(new { message = "Cliente eliminado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}