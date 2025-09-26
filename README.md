# 📦 Sistema de Inventario

Sistema básico de inventario con módulos de compras, ventas y reporte de stock, desarrollado con **ASP.NET Core Web API** como backend y **Blazor Server** como frontend.

## Características

- ✅ **Arquitectura Clean Architecture** con separación de responsabilidades
- ✅ **Autenticación JWT** con roles (Administrador/Vendedor)
- ✅ **CRUD completo** para Productos, Proveedores y Clientes
- ✅ **Control de Stock** con alertas de stock mínimo
- ✅ **Interfaz moderna** con MudBlazor UI Framework
- ✅ **Base de datos PostgreSQL** con Entity Framework Core
- ✅ **Responsive Design** adaptable a diferentes dispositivos
- ✅ **Validaciones** del lado cliente y servidor
- ✅ **Manejo de errores** centralizado

## Arquitectura

```
InventorySystemNew/
├── InventorySystem.Core/          # Entidades de dominio y contratos
├── InventorySystem.Infrastructure/ # Acceso a datos y servicios
├── InventorySystem.API/           # Controladores Web API
├── InventorySystem.Web/           # Aplicación Blazor Server
└── InventorySystem.sln            # Solución principal
```

### Tecnologías Utilizadas

**Backend:**
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- PostgreSQL (Npgsql)
- JWT Authentication
- AutoMapper
- FluentValidation

**Frontend:**
- Blazor Server 8.0
- MudBlazor UI Framework
- Blazored LocalStorage
- HttpClient Factory

**Base de Datos:**
- PostgreSQL 13+
- Railway Cloud Database

## 🛠️ Configuración e Instalación

### Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (local o Railway)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### 1. Clonar el Repositorio

```bash
git clone <repository-url>
cd InventorySystemNew
```

### 2. Configurar Base de Datos

#### Opción A: PostgreSQL Local
```json
// InventorySystem.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=InventoryDB;Username=postgres;Password=tu_password"
  }
}
```

#### Opción B: Railway PostgreSQL (Recomendado)
```json
// InventorySystem.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=junction.proxy.rlwy.net;Port=44468;Database=railway;Username=postgres;Password=tu_railway_password"
  }
}
```

### 3. Configurar API Base URL

```json
// InventorySystem.Web/appsettings.json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7121/"
  }
}
```

### 4. Restaurar Paquetes

```bash
dotnet restore
```

### 5. Ejecutar Migraciones

```bash
cd InventorySystem.API
dotnet ef database update
```

### 6. Ejecutar la Aplicación

#### Terminal 1 - API Backend:
```bash
cd InventorySystem.API
dotnet run
```
API disponible en: `https://localhost:7121` o `http://localhost:5113`

#### Terminal 2 - Frontend Blazor:
```bash
cd InventorySystem.Web
dotnet run
```
Aplicación web disponible en: `http://localhost:5104`

##  Uso del Sistema

### 1. Registro de Usuario
- Accede a `/register`
- Completa el formulario con tus datos
- Selecciona el rol: **Admin** o **Vendedor**

### 2. Iniciar Sesión
- Ve a `/login`
- Ingresa email y contraseña
- Serás redirigido al dashboard

### 3. Gestión de Productos
- **Crear**: Click en "Nuevo Producto"
- **Editar**: Click en el ícono de edición
- **Eliminar**: Click en el ícono de eliminación
- **Buscar**: Usa la barra de búsqueda

### 4. Gestión de Proveedores y Clientes
- Funcionalidad similar a productos
- Campos específicos para información de contacto

##  Sistema de Autenticación

### Roles Disponibles

| Rol | Permisos |
|-----|----------|
| **Admin** | Acceso completo a todas las funcionalidades |
| **Vendedor** | Acceso a productos, proveedores, clientes y ventas |

### Endpoints Protegidos

```csharp
[Authorize(Roles = "Admin,Vendedor")]
public class ProductosController : ControllerBase

[Authorize(Roles = "Admin")]
public class ReportesController : ControllerBase
```

##  API Endpoints

### Autenticación
```
POST /api/auth/register    # Registro de usuario
POST /api/auth/login       # Inicio de sesión
POST /api/auth/logout      # Cerrar sesión
```

### Productos
```
GET    /api/productos      # Obtener todos los productos
GET    /api/productos/{id} # Obtener producto por ID
POST   /api/productos      # Crear producto
PUT    /api/productos/{id} # Actualizar producto
DELETE /api/productos/{id} # Eliminar producto
```

### Proveedores
```
GET    /api/proveedores      # Obtener todos los proveedores
GET    /api/proveedores/{id} # Obtener proveedor por ID
POST   /api/proveedores      # Crear proveedor
PUT    /api/proveedores/{id} # Actualizar proveedor
DELETE /api/proveedores/{id} # Eliminar proveedor
```

### Clientes
```
GET    /api/clientes      # Obtener todos los clientes
GET    /api/clientes/{id} # Obtener cliente por ID
POST   /api/clientes      # Crear cliente
PUT    /api/clientes/{id} # Actualizar cliente
DELETE /api/clientes/{id} # Eliminar cliente
```

##  Modelo de Base de Datos

### Entidades Principales

**ApplicationUser**
- Id, Email, FirstName, LastName
- Roles de usuario

**Producto**
- Id, Codigo, Nombre, Descripcion
- Precio, Stock, StockMinimo
- FechaCreacion, Activo

**Proveedor**
- Id, Nombre, Ruc, Direccion
- Contacto, Telefono, Email
- FechaCreacion, Activo

**Cliente**
- Id, Nombre, Direccion
- Telefono, Email
- FechaCreacion, Activo

**Compra / CompraDetalle**
- Sistema de compras con detalles
- Actualización automática de stock

**Venta / VentaDetalle**
- Sistema de ventas con detalles
- Control de inventario


##  Solución de Problemas

### Error de Conexión a Base de Datos
```bash
# Verificar que PostgreSQL esté ejecutándose
systemctl status postgresql

# Verificar string de conexión
dotnet ef database update --verbose
```

### Error de CORS en Desarrollo
```csharp
// Program.cs - API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Problemas de Autenticación JWT
1. Verificar que el token no haya expirado
2. Comprobar la configuración de JWT en `appsettings.json`
3. Limpiar localStorage del navegador
5