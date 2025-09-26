# 🚀 Guía de Despliegue

Esta guía te ayudará a desplegar el Sistema de Inventario en diferentes entornos de producción.

## 🌐 Opciones de Despliegue

### 1. **Railway (Recomendado)**
### 2. **Azure App Service**
### 3. **AWS Elastic Beanstalk**
### 4. **Docker Containers**
### 5. **IIS (Windows Server)**

## 🚂 Despliegue en Railway

Railway es una plataforma de despliegue moderna que simplifica el proceso de hosting.

### Paso 1: Preparación del Proyecto

```bash
# 1. Crear railway.json en la raíz del proyecto
{
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet InventorySystem.API.dll",
    "healthcheckPath": "/health"
  }
}
```

### Paso 2: Configurar Variables de Entorno

En el dashboard de Railway, configura:

```env
# Base de datos
DATABASE_URL=postgresql://usuario:password@host:puerto/database

# JWT Configuration  
JWT_SECRET=tu-super-secreto-jwt-de-al-menos-32-caracteres
JWT_ISSUER=InventorySystem
JWT_AUDIENCE=InventorySystem.Users
JWT_EXPIRATION_HOURS=24

# Environment
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Opcional: Configuración adicional
CORS_ORIGINS=https://tu-dominio.com
```

### Paso 3: Configurar appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  },
  "JwtSettings": {
    "Secret": "${JWT_SECRET}",
    "Issuer": "${JWT_ISSUER}",
    "Audience": "${JWT_AUDIENCE}",
    "ExpirationHours": "${JWT_EXPIRATION_HOURS}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Paso 4: Desplegar

```bash
# Conectar con Railway CLI
npm install -g @railway/cli
railway login

# Inicializar proyecto
railway init

# Desplegar
railway up
```

## ☁️ Despliegue en Azure

### Paso 1: Crear Recursos Azure

```bash
# Azure CLI
az login

# Crear grupo de recursos
az group create --name rg-inventory --location "East US"

# Crear App Service Plan
az appservice plan create --name plan-inventory --resource-group rg-inventory --sku B1

# Crear Web App
az webapp create --name inventory-api --resource-group rg-inventory --plan plan-inventory --runtime "DOTNET|8.0"

# Crear Azure Database for PostgreSQL
az postgres server create --resource-group rg-inventory --name inventory-db --location "East US" --admin-user adminuser --admin-password "Password123!" --sku-name GP_Gen5_2
```

### Paso 2: Configurar App Settings

```bash
# Configurar connection string
az webapp config connection-string set --name inventory-api --resource-group rg-inventory --connection-string-type PostgreSQL --settings DefaultConnection="Host=inventory-db.postgres.database.azure.com;Database=inventory;Username=adminuser@inventory-db;Password=Password123!;SslMode=Require"

# Configurar app settings
az webapp config appsettings set --name inventory-api --resource-group rg-inventory --settings JWT_SECRET="tu-secreto-jwt-muy-largo-y-seguro" ASPNETCORE_ENVIRONMENT="Production"
```

### Paso 3: Desplegar con GitHub Actions

```yaml
# .github/workflows/deploy-azure.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Publish
      run: dotnet publish InventorySystem.API/InventorySystem.API.csproj -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'inventory-api'
        slot-name: 'production'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

## 🐳 Despliegue con Docker

### Paso 1: Crear Dockerfile para API

```dockerfile
# InventorySystem.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventorySystem.API/InventorySystem.API.csproj", "InventorySystem.API/"]
COPY ["InventorySystem.Infrastructure/InventorySystem.Infrastructure.csproj", "InventorySystem.Infrastructure/"]
COPY ["InventorySystem.Core/InventorySystem.Core.csproj", "InventorySystem.Core/"]
RUN dotnet restore "InventorySystem.API/InventorySystem.API.csproj"
COPY . .
WORKDIR "/src/InventorySystem.API"
RUN dotnet build "InventorySystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventorySystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.API.dll"]
```

### Paso 2: Crear Dockerfile para Web

```dockerfile
# InventorySystem.Web/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventorySystem.Web/InventorySystem.Web.csproj", "InventorySystem.Web/"]
RUN dotnet restore "InventorySystem.Web/InventorySystem.Web.csproj"
COPY . .
WORKDIR "/src/InventorySystem.Web"
RUN dotnet build "InventorySystem.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventorySystem.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.Web.dll"]
```

### Paso 3: Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:13
    environment:
      POSTGRES_DB: inventory
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build:
      context: .
      dockerfile: InventorySystem.API/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=inventory;Username=postgres;Password=password123
      - JwtSettings__Secret=tu-secreto-jwt-muy-largo-y-seguro-para-produccion
    ports:
      - "5000:80"
    depends_on:
      - postgres

  web:
    build:
      context: .
      dockerfile: InventorySystem.Web/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ApiSettings__BaseUrl=http://api:80/
    ports:
      - "5001:80"
    depends_on:
      - api

volumes:
  postgres_data:
```

### Paso 4: Ejecutar con Docker

```bash
# Construir y ejecutar
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Ejecutar migraciones
docker-compose exec api dotnet ef database update

# Parar servicios
docker-compose down
```

## 🗄️ Configuración de Base de Datos

### PostgreSQL en Producción

```sql
-- Crear base de datos
CREATE DATABASE inventory_prod;

-- Crear usuario específico para la aplicación
CREATE USER inventory_user WITH ENCRYPTED PASSWORD 'secure_password_123';

-- Otorgar permisos
GRANT CONNECT ON DATABASE inventory_prod TO inventory_user;
GRANT USAGE ON SCHEMA public TO inventory_user;
GRANT CREATE ON SCHEMA public TO inventory_user;
```

### Configuración de Performance

```sql
-- Índices recomendados
CREATE INDEX idx_productos_codigo ON productos(codigo);
CREATE INDEX idx_productos_nombre ON productos(nombre);
CREATE INDEX idx_productos_activo ON productos(activo);
CREATE INDEX idx_compras_fecha ON compras(fecha);
CREATE INDEX idx_ventas_fecha ON ventas(fecha);
CREATE INDEX idx_compras_proveedor ON compras(proveedor_id);
CREATE INDEX idx_ventas_cliente ON ventas(cliente_id);
```

### Backup y Restore

```bash
# Backup
pg_dump -h hostname -U username -d database_name > backup.sql

# Restore
psql -h hostname -U username -d database_name < backup.sql

# Backup automático con cron
0 2 * * * pg_dump -h hostname -U username -d inventory_prod > /backups/inventory_$(date +\%Y\%m\%d).sql
```

## 🔍 Monitoreo y Logging

### Application Insights (Azure)

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// appsettings.Production.json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key"
  }
}
```

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

### Structured Logging

```csharp
// Program.cs
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddApplicationInsights();
});

// En servicios
private readonly ILogger<ProductoService> _logger;

public async Task<ProductoDto> CreateAsync(ProductoDto productoDto)
{
    _logger.LogInformation("Creando producto: {Nombre}", productoDto.Nombre);
    try
    {
        // lógica
        _logger.LogInformation("Producto creado exitosamente: {Id}", result.Id);
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creando producto: {Nombre}", productoDto.Nombre);
        throw;
    }
}
```

## 🔐 Seguridad en Producción

### HTTPS Enforcement

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

### Security Headers

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

### Rate Limiting

```csharp
// Instalar: AspNetCoreRateLimit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

app.UseIpRateLimiting();
```

## 📊 Performance Optimization

### Caching

```csharp
// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// In-Memory Cache
builder.Services.AddMemoryCache();

// En servicios
private readonly IMemoryCache _cache;

public async Task<List<ProductoDto>> GetAllAsync()
{
    const string cacheKey = "productos_all";
    
    if (!_cache.TryGetValue(cacheKey, out List<ProductoDto> productos))
    {
        productos = await _repository.GetAllAsync();
        _cache.Set(cacheKey, productos, TimeSpan.FromMinutes(15));
    }
    
    return productos;
}
```

### Database Optimization

```csharp
// Program.cs - Connection Pooling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
        npgsqlOptions.CommandTimeout(30);
    });
});

// Query optimization
public async Task<List<ProductoDto>> GetProductosWithStockAsync()
{
    return await _context.Productos
        .Where(p => p.Stock > 0)
        .Select(p => new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Stock = p.Stock
        })
        .ToListAsync();
}
```

## 🚨 Troubleshooting

### Problemas Comunes

1. **Error de conexión a base de datos**
   ```bash
   # Verificar conectividad
   telnet hostname port
   
   # Verificar string de conexión
   dotnet ef database update --verbose
   ```

2. **Error de memoria en producción**
   ```csharp
   // Configurar límites de memoria
   builder.Services.Configure<GCSettings>(options =>
   {
       options.GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
   });
   ```

3. **Timeouts de HTTP**
   ```csharp
   builder.Services.AddHttpClient("InventoryAPI", client =>
   {
       client.Timeout = TimeSpan.FromSeconds(120);
   });
   ```

### Monitoring Dashboard

```bash
# Métricas básicas con curl
curl -f http://localhost:5000/health || exit 1

# Logs con Docker
docker-compose logs -f --tail=100 api

# Memoria y CPU
docker stats
```

¡Tu aplicación está lista para producción! 🎉