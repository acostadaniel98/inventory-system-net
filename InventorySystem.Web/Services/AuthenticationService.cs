using InventorySystem.Web.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace InventorySystem.Web.Services
{
    /// <summary>
    /// Interfaz para el servicio de autenticación
    /// </summary>
    public interface IAuthenticationService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync();
        Task<UserInfo> GetCurrentUserAsync();
    }

    /// <summary>
    /// Servicio de autenticación personalizado
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthenticationService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("InventoryAPI");
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync("authToken", authResponse.Token);
                        await _localStorage.SetItemAsync("userInfo", authResponse);
                        
                        // Notificar cambio de estado de autenticación
                        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
                        
                        return new ApiResponse<AuthResponseDto> 
                        { 
                            Success = true, 
                            Data = authResponse 
                        };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                var errorMessage = errorResponse?.ContainsKey("message") == true 
                    ? errorResponse["message"].ToString() 
                    : "Error al iniciar sesión";

                return new ApiResponse<AuthResponseDto> 
                { 
                    Success = false, 
                    Message = errorMessage 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto> 
                { 
                    Success = false, 
                    Message = "Error de conexión con el servidor" 
                };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync("authToken", authResponse.Token);
                        await _localStorage.SetItemAsync("userInfo", authResponse);
                        
                        // Notificar cambio de estado de autenticación
                        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
                        
                        return new ApiResponse<AuthResponseDto> 
                        { 
                            Success = true, 
                            Data = authResponse 
                        };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                var errorMessage = errorResponse?.ContainsKey("message") == true 
                    ? errorResponse["message"].ToString() 
                    : "Error al registrar usuario";

                return new ApiResponse<AuthResponseDto> 
                { 
                    Success = false, 
                    Message = errorMessage 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto> 
                { 
                    Success = false, 
                    Message = "Error de conexión con el servidor" 
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userInfo");
            
            // Notificar cambio de estado de autenticación
            ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            try
            {
                var userInfo = await _localStorage.GetItemAsync<AuthResponseDto>("userInfo");
                if (userInfo != null)
                {
                    return new UserInfo
                    {
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Roles = userInfo.Roles,
                        IsAuthenticated = true
                    };
                }
            }
            catch { }

            return new UserInfo { IsAuthenticated = false };
        }
    }

    /// <summary>
    /// Proveedor de estado de autenticación personalizado
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                var userInfo = await _localStorage.GetItemAsync<AuthResponseDto>("userInfo");

                if (string.IsNullOrEmpty(token) || userInfo == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Verificar si el token ha expirado
                if (userInfo.ExpireDate <= DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    await _localStorage.RemoveItemAsync("userInfo");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim("FirstName", userInfo.FirstName),
                    new Claim("LastName", userInfo.LastName),
                    new Claim(ClaimTypes.Name, $"{userInfo.FirstName} {userInfo.LastName}")
                };

                claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}