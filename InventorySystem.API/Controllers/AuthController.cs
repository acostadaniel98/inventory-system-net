using Microsoft.AspNetCore.Mvc;
using InventorySystem.Core.DTOs;
using InventorySystem.Core.Interfaces;

namespace InventorySystem.API.Controllers
{
    /// <summary>
    /// Controlador para la autenticación de usuarios
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        /// <param name="loginDto">Datos de inicio de sesión</param>
        /// <returns>Token de acceso y datos del usuario</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("Usuario {Email} ha iniciado sesión exitosamente", loginDto.Email);
                
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento de inicio de sesión fallido para {Email}: {Message}", loginDto.Email, ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar inicio de sesión para {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        /// <param name="registerDto">Datos del nuevo usuario</param>
        /// <returns>Token de acceso y datos del usuario registrado</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("Nuevo usuario registrado: {Email}", registerDto.Email);
                
                return CreatedAtAction(nameof(Register), response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al registrar usuario {Email}: {Message}", registerDto.Email, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario {Email}", registerDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cerrar sesión
        /// </summary>
        /// <returns>Confirmación de cierre de sesión</returns>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
                if (userId != null)
                {
                    await _authService.LogoutAsync(userId);
                    _logger.LogInformation("Usuario {UserId} ha cerrado sesión", userId);
                }
                
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cambiar contraseña
        /// </summary>
        /// <param name="changePasswordDto">Datos para cambio de contraseña</param>
        /// <returns>Confirmación del cambio de contraseña</returns>
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
                if (userId == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result)
                {
                    return BadRequest(new { message = "Error al cambiar la contraseña" });
                }

                _logger.LogInformation("Usuario {UserId} ha cambiado su contraseña", userId);
                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verificar si el token es válido
        /// </summary>
        /// <returns>Información del usuario autenticado</returns>
        [HttpGet("verify-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyToken()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
                var email = User.FindFirst("email")?.Value;
                var firstName = User.FindFirst("FirstName")?.Value;
                var lastName = User.FindFirst("LastName")?.Value;
                var roles = User.FindAll("role").Select(c => c.Value).ToList();

                if (userId == null)
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                return Ok(new
                {
                    userId,
                    email,
                    firstName,
                    lastName,
                    roles,
                    isValid = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}