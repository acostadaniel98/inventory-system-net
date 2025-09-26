# 🏗️ Arquitectura del Sistema

## Visión General

El Sistema de Inventario está construido usando **Clean Architecture** con separación clara de responsabilidades entre capas. La arquitectura permite escalabilidad, mantenibilidad y testabilidad del código.

## 📊 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
├─────────────────────────────────────────────────────────────┤
│  InventorySystem.Web (Blazor Server)                       │
│  - Páginas Razor Components                                 │
│  - Servicios de Cliente HTTP                               │
│  - Autenticación JWT                                       │
│  - UI con MudBlazor                                        │
├─────────────────────────────────────────────────────────────┤
│  InventorySystem.API (Web API Controllers)                 │
│  - Controladores REST API                                  │
│  - Middleware de autenticación                             │
│  - Validación de modelos                                   │
│  - Swagger/OpenAPI                                         │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                         │
├─────────────────────────────────────────────────────────────┤
│  InventorySystem.Infrastructure                            │
│  - Servicios de Aplicación                                │
│  - Implementación de repositorios                         │
│  - Servicios de autenticación                             │
│  - Configuración EF Core                                  │
│  - Mapeo con AutoMapper                                   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                           │
├─────────────────────────────────────────────────────────────┤
│  InventorySystem.Core                                      │
│  - Entidades de dominio                                    │
│  - Interfaces de repositorios                             │
│  - Interfaces de servicios                                │
│  - DTOs y modelos                                         │
│  - Lógica de negocio pura                                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                      │
├─────────────────────────────────────────────────────────────┤
│  Base de Datos PostgreSQL                                  │
│  - Tablas normalizadas                                     │
│  - Índices optimizados                                     │
│  - Constraints de integridad                               │
│  - Migraciones EF Core                                     │
└─────────────────────────────────────────────────────────────┘
```

## 🏛️ Capas de la Arquitectura

### 1. **Domain Layer (Core)**
**Propósito**: Contiene la lógica de negocio pura y las reglas de dominio.

**Componentes:**
- **Entidades**: `Producto`, `Proveedor`, `Cliente`, `Compra`, `Venta`, `ApplicationUser`
- **Interfaces**: `IProductoRepository`, `IAuthService`, `ICompraService`
- **DTOs**: Modelos de transferencia de datos
- **Enums**: Estados y tipos del sistema

**Características:**
- ✅ Sin dependencias externas
- ✅ Lógica de negocio centralizada
- ✅ Reglas de validación de dominio
- ✅ Interfaces para inversión de dependencias

### 2. **Application Layer (Infrastructure)**
**Propósito**: Implementa los casos de uso y coordina entre el dominio y la infraestructura.

**Componentes:**
- **Servicios**: `AuthService`, `ProductoService`, `CompraService`, `VentaService`
- **Repositorios**: Implementaciones concretas con EF Core
- **Context**: `ApplicationDbContext` para EF Core
- **Mappers**: Configuración de AutoMapper

**Características:**
- ✅ Implementa interfaces del dominio
- ✅ Coordina operaciones complejas
- ✅ Maneja transacciones
- ✅ Aplica reglas de negocio

### 3. **Presentation Layer (API + Web)**
**Propósito**: Expone la funcionalidad a través de APIs REST y una interfaz web.

**API (InventorySystem.API):**
- **Controladores**: `AuthController`, `ProductosController`, `ProveedoresController`
- **Middleware**: Autenticación JWT, manejo de errores
- **Configuración**: Swagger, CORS, servicios DI

**Web (InventorySystem.Web):**
- **Páginas**: Componentes Blazor Server
- **Servicios**: Clientes HTTP para consumir API
- **Autenticación**: Estado de autenticación personalizado

## 🔄 Flujo de Datos

### Operación CRUD Típica
```
1. Usuario → Blazor Component
2. Component → HTTP Client Service  
3. HTTP Service → API Controller
4. Controller → Application Service
5. Service → Repository
6. Repository → EF Core Context
7. Context → PostgreSQL Database
```

### Ejemplo: Crear Producto
```csharp
// 1. Usuario envía formulario en ProductoDialog.razor
await ProductoService.CreateAsync(producto);

// 2. ProductoClientService hace HTTP POST
var response = await _httpClient.PostAsJsonAsync("api/productos", producto);

// 3. ProductosController recibe la petición
[HttpPost]
public async Task<ActionResult<ProductoDto>> Create([FromBody] ProductoDto productoDto)

// 4. Controller usa el Application Service
var result = await _productoService.CreateAsync(productoDto);

// 5. Service mapea y persiste
var producto = _mapper.Map<Producto>(productoDto);
await _repository.AddAsync(producto);

// 6. Repository usa EF Context
_context.Productos.Add(producto);
await _context.SaveChangesAsync();
```

## 🛠️ Patrones de Diseño Implementados

### 1. **Repository Pattern**
```csharp
public interface IProductoRepository
{
    Task<List<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<Producto> AddAsync(Producto producto);
    Task<Producto> UpdateAsync(Producto producto);
    Task DeleteAsync(int id);
}
```

### 2. **Service Pattern**
```csharp
public interface IProductoService
{
    Task<List<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<ProductoDto> CreateAsync(ProductoDto productoDto);
    Task<ProductoDto> UpdateAsync(int id, ProductoDto productoDto);
    Task DeleteAsync(int id);
}
```

### 3. **Unit of Work Pattern**
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    // ... más DbSets

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}
```

### 4. **Factory Pattern**
```csharp
// En Program.cs para HTTP Clients
builder.Services.AddHttpClient("InventoryAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## 🔐 Seguridad por Capas

### 1. **Presentation Layer Security**
- HTTPS enforced
- CORS configurado
- Input validation
- XSS protection

### 2. **Application Layer Security**
- JWT token validation
- Role-based authorization
- Business rule enforcement
- Data sanitization

### 3. **Data Layer Security**
- SQL injection prevention (EF Core)
- Connection string encryption
- Database user permissions
- Audit logging

## 📊 Modelo de Datos

### Entidades Principales
```csharp
// Core Domain Entities
public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    // ... más propiedades
}

public class Compra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; }
    public List<CompraDetalle> CompraDetalles { get; set; }
    // ... más propiedades
}
```

### Relaciones
- **Producto** → **CompraDetalle** (One-to-Many)
- **Producto** → **VentaDetalle** (One-to-Many)
- **Proveedor** → **Compra** (One-to-Many)
- **Cliente** → **Venta** (One-to-Many)
- **ApplicationUser** → **Compra/Venta** (One-to-Many)

## 🚀 Escalabilidad y Performance

### Optimizaciones Implementadas
- **Async/Await**: Operaciones no bloqueantes
- **Lazy Loading**: Carga diferida de relaciones
- **Connection Pooling**: Reutilización de conexiones
- **HTTP Client Factory**: Gestión eficiente de HTTP clients

### Preparado para Escalar
- **Microservicios**: Arquitectura permite separación en servicios
- **Caching**: Redis ready para caché distribuido  
- **Load Balancing**: API stateless ready para balanceadores
- **Database Sharding**: Modelo permite particionamiento

## 🧪 Testabilidad

### Unit Testing
```csharp
[Test]
public async Task CreateProducto_ValidData_ReturnsCreatedProducto()
{
    // Arrange
    var mockRepo = new Mock<IProductoRepository>();
    var service = new ProductoService(mockRepo.Object);
    
    // Act & Assert
    var result = await service.CreateAsync(productoDto);
    Assert.That(result, Is.Not.Null);
}
```

### Integration Testing
- TestServer para API testing
- In-memory database para tests
- MockHttp para client testing

## 🔄 CI/CD Ready

### Deployment Pipeline
1. **Build**: dotnet build
2. **Test**: dotnet test
3. **Package**: dotnet publish
4. **Deploy**: Docker containers
5. **Monitor**: Health checks

### Environment Configuration
- Development: Local PostgreSQL
- Staging: Railway PostgreSQL
- Production: Managed PostgreSQL

Esta arquitectura garantiza un sistema robusto, mantenible y escalable para el manejo de inventarios empresariales.