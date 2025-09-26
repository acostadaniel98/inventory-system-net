using InventorySystem.Web.Components;
using InventorySystem.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar MudBlazor
builder.Services.AddMudServices();

// Configurar LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configurar HttpClient para la API
builder.Services.AddHttpClient("InventoryAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7121/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicios de autenticación personalizados
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Servicios de negocio
builder.Services.AddScoped<IProductoClientService, ProductoClientService>();
builder.Services.AddScoped<IProveedorClientService, ProveedorClientService>();
builder.Services.AddScoped<IClienteClientService, ClienteClientService>();
builder.Services.AddScoped<ICompraClientService, CompraClientService>();
builder.Services.AddScoped<IVentaClientService, VentaClientService>();
builder.Services.AddScoped<IReporteClientService, ReporteClientService>();

// Configuración de autorización
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
