using InventorySystem.Web.Models;
using Blazored.LocalStorage;

namespace InventorySystem.Web.Services
{
    /// <summary>
    /// Interfaz para el servicio de compras
    /// </summary>
    public interface ICompraClientService
    {
        Task<ApiResponse<List<CompraDto>>> GetAllAsync();
        Task<ApiResponse<CompraDto>> GetByIdAsync(int id);
        Task<ApiResponse<CompraDto>> CreateAsync(CompraDto compra);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    /// <summary>
    /// Interfaz para el servicio de ventas
    /// </summary>
    public interface IVentaClientService
    {
        Task<ApiResponse<List<VentaDto>>> GetAllAsync();
        Task<ApiResponse<VentaDto>> GetByIdAsync(int id);
        Task<ApiResponse<VentaDto>> CreateAsync(VentaDto venta);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    /// <summary>
    /// Interfaz para el servicio de reportes
    /// </summary>
    public interface IReporteClientService
    {
        Task<ApiResponse<StockReportDto>> GetStockReporteAsync();
        Task<ApiResponse<VentasReportDto>> GetVentasReporteAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<DashboardStatsDto>> GetDashboardDataAsync();
    }

    /// <summary>
    /// Servicio de compras para el cliente
    /// </summary>
    public class CompraClientService : BaseApiService, ICompraClientService
    {
        public CompraClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<List<CompraDto>>> GetAllAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/compras");
            return await HandleResponseAsync<List<CompraDto>>(response);
        }

        public async Task<ApiResponse<CompraDto>> GetByIdAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/compras/{id}");
            return await HandleResponseAsync<CompraDto>(response);
        }

        public async Task<ApiResponse<CompraDto>> CreateAsync(CompraDto compra)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/compras", compra);
            return await HandleResponseAsync<CompraDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/compras/{id}");
            var result = await HandleResponseAsync<object>(response);
            return new ApiResponse<bool> { Success = result.Success, Data = result.Success, Message = result.Message };
        }
    }

    /// <summary>
    /// Servicio de ventas para el cliente
    /// </summary>
    public class VentaClientService : BaseApiService, IVentaClientService
    {
        public VentaClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<List<VentaDto>>> GetAllAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/ventas");
            return await HandleResponseAsync<List<VentaDto>>(response);
        }

        public async Task<ApiResponse<VentaDto>> GetByIdAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/ventas/{id}");
            return await HandleResponseAsync<VentaDto>(response);
        }

        public async Task<ApiResponse<VentaDto>> CreateAsync(VentaDto venta)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/ventas", venta);
            return await HandleResponseAsync<VentaDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/ventas/{id}");
            var result = await HandleResponseAsync<object>(response);
            return new ApiResponse<bool> { Success = result.Success, Data = result.Success, Message = result.Message };
        }
    }

    /// <summary>
    /// Servicio de reportes para el cliente
    /// </summary>
    public class ReporteClientService : BaseApiService, IReporteClientService
    {
        public ReporteClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<StockReportDto>> GetStockReporteAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/reportes/stock");
            return await HandleResponseAsync<StockReportDto>(response);
        }

        public async Task<ApiResponse<VentasReportDto>> GetVentasReporteAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/reportes/ventas?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}");
            return await HandleResponseAsync<VentasReportDto>(response);
        }

        public async Task<ApiResponse<DashboardStatsDto>> GetDashboardDataAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/reportes/dashboard");
            return await HandleResponseAsync<DashboardStatsDto>(response);
        }
    }
}