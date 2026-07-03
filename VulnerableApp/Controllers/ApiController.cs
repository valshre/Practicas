using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
using System.Linq; 
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger;
        
        public ApiController(AppDbContext db, ILogger<ApiController> logger) 
        { 
            _db = db; 
            _logger = logger;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio ApiController.GetUser | Usuario: {Usuario} | IP: {IP} | Param id: {Id}", usuario, ip, id);

            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (!currentUserId.HasValue) 
                {
                    _logger.LogWarning("Intento de acceso no autorizado a la API GetUser desde IP: {IP}", ip);
                    return Unauthorized();
                }
                
                if (id != currentUserId.Value) 
                {
                    _logger.LogWarning("El usuario {Usuario} intentó acceder a los datos de otro id ({Id})", usuario, id);
                    return Forbid();
                }
                
                var user = _db.Users.Find(id);
                if (user == null) return NotFound();
                
                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ApiController.GetUser para el usuario {Usuario}", usuario);
                return StatusCode(500, "Error interno del servidor");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin ApiController.GetUser | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio ApiController.GetAllUsers | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
               
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (!currentUserId.HasValue) 
                {
                    _logger.LogWarning("Intento de acceso no autorizado a la API GetAllUsers desde IP: {IP}", ip);
                    return Unauthorized();
                }

                var safeUsers = _db.Users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email
                }).ToList();

                return Ok(safeUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ApiController.GetAllUsers para el usuario {Usuario}", usuario);
                return StatusCode(500, "Error interno del servidor");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin ApiController.GetAllUsers | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}