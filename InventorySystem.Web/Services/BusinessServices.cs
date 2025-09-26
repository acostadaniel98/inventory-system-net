using InventorySystem.Web.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InventorySystem.Web.Services
{
    /// <summary>
    /// Clase base para servicios de API
    /// </summary>
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILocalStorageService _localStorage;

        protected BaseApiService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("InventoryAPI");
            _localStorage = localStorage;
        }

        protected async Task SetAuthorizationHeaderAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected async Task<ApiResponse<T>> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(content) || content == "null")
                    {
                        return new ApiResponse<T> { Success = true, Data = default };
                    }

                    var data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                    
                    return new ApiResponse<T> { Success = true, Data = data };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = "Error desconocido";

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    errorMessage = errorResponse?.ContainsKey("message") == true 
                        ? errorResponse["message"].ToString() ?? errorMessage
                        : errorMessage;
                }
                catch { }

                return new ApiResponse<T> 
                { 
                    Success = false, 
                    Message = errorMessage 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> 
                { 
                    Success = false, 
                    Message = "Error de conexión con el servidor" 
                };
            }
        }
    }

    /// <summary>
    /// Interfaz para el servicio de productos
    /// </summary>
    public interface IProductoClientService
    {
        Task<ApiResponse<List<ProductoDto>>> GetAllAsync();
        Task<ApiResponse<ProductoDto>> GetByIdAsync(int id);
        Task<ApiResponse<ProductoDto>> CreateAsync(ProductoDto producto);
        Task<ApiResponse<ProductoDto>> UpdateAsync(int id, ProductoDto producto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    /// <summary>
    /// Servicio de productos para el cliente
    /// </summary>
    public class ProductoClientService : BaseApiService, IProductoClientService
    {
        public ProductoClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<List<ProductoDto>>> GetAllAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/productos");
            return await HandleResponseAsync<List<ProductoDto>>(response);
        }

        public async Task<ApiResponse<ProductoDto>> GetByIdAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/productos/{id}");
            return await HandleResponseAsync<ProductoDto>(response);
        }

        public async Task<ApiResponse<ProductoDto>> CreateAsync(ProductoDto producto)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/productos", producto);
            return await HandleResponseAsync<ProductoDto>(response);
        }

        public async Task<ApiResponse<ProductoDto>> UpdateAsync(int id, ProductoDto producto)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/productos/{id}", producto);
            return await HandleResponseAsync<ProductoDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/productos/{id}");
            var result = await HandleResponseAsync<object>(response);
            return new ApiResponse<bool> { Success = result.Success, Data = result.Success, Message = result.Message };
        }
    }

    /// <summary>
    /// Interfaz para el servicio de proveedores
    /// </summary>
    public interface IProveedorClientService
    {
        Task<ApiResponse<List<ProveedorDto>>> GetAllAsync();
        Task<ApiResponse<ProveedorDto>> GetByIdAsync(int id);
        Task<ApiResponse<ProveedorDto>> CreateAsync(ProveedorDto proveedor);
        Task<ApiResponse<ProveedorDto>> UpdateAsync(int id, ProveedorDto proveedor);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    /// <summary>
    /// Servicio de proveedores para el cliente
    /// </summary>
    public class ProveedorClientService : BaseApiService, IProveedorClientService
    {
        public ProveedorClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<List<ProveedorDto>>> GetAllAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/proveedores");
            return await HandleResponseAsync<List<ProveedorDto>>(response);
        }

        public async Task<ApiResponse<ProveedorDto>> GetByIdAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/proveedores/{id}");
            return await HandleResponseAsync<ProveedorDto>(response);
        }

        public async Task<ApiResponse<ProveedorDto>> CreateAsync(ProveedorDto proveedor)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/proveedores", proveedor);
            return await HandleResponseAsync<ProveedorDto>(response);
        }

        public async Task<ApiResponse<ProveedorDto>> UpdateAsync(int id, ProveedorDto proveedor)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/proveedores/{id}", proveedor);
            return await HandleResponseAsync<ProveedorDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/proveedores/{id}");
            var result = await HandleResponseAsync<object>(response);
            return new ApiResponse<bool> { Success = result.Success, Data = result.Success, Message = result.Message };
        }
    }

    /// <summary>
    /// Interfaz para el servicio de clientes
    /// </summary>
    public interface IClienteClientService
    {
        Task<ApiResponse<List<ClienteDto>>> GetAllAsync();
        Task<ApiResponse<ClienteDto>> GetByIdAsync(int id);
        Task<ApiResponse<ClienteDto>> CreateAsync(ClienteDto cliente);
        Task<ApiResponse<ClienteDto>> UpdateAsync(int id, ClienteDto cliente);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    /// <summary>
    /// Servicio de clientes para el cliente web
    /// </summary>
    public class ClienteClientService : BaseApiService, IClienteClientService
    {
        public ClienteClientService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage) 
            : base(httpClientFactory, localStorage) { }

        public async Task<ApiResponse<List<ClienteDto>>> GetAllAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("api/clientes");
            return await HandleResponseAsync<List<ClienteDto>>(response);
        }

        public async Task<ApiResponse<ClienteDto>> GetByIdAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"api/clientes/{id}");
            return await HandleResponseAsync<ClienteDto>(response);
        }

        public async Task<ApiResponse<ClienteDto>> CreateAsync(ClienteDto cliente)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/clientes", cliente);
            return await HandleResponseAsync<ClienteDto>(response);
        }

        public async Task<ApiResponse<ClienteDto>> UpdateAsync(int id, ClienteDto cliente)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/clientes/{id}", cliente);
            return await HandleResponseAsync<ClienteDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/clientes/{id}");
            var result = await HandleResponseAsync<object>(response);
            return new ApiResponse<bool> { Success = result.Success, Data = result.Success, Message = result.Message };
        }
    }
}