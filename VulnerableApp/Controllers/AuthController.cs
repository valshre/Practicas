using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VulnerableApp.Data;
using System;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(AppDbContext db, ILogger<AuthController> logger) 
        { 
            _db = db;
            _logger = logger;
        }
        
        public IActionResult Login()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Login (GET) | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
            return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista de Login");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Login (GET) | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
        
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            // Log de entrada OMITIENDO la contraseña
            _logger.LogInformation("Inicio AuthController.Login (POST) | Intento de usuario: {Username} | IP: {IP} | Password: [REDACTED]", username, ip);

            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Username == username);
              
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("EVENTO AUTH: Credenciales inválidas para el intento de acceso del usuario '{Username}' desde la IP {IP}", username, ip);
                    ViewBag.Error = "Credenciales inválidas";
                    return View();
                }

                _logger.LogInformation("EVENTO AUTH: Inicio de sesión exitoso para el usuario '{Username}' desde la IP {IP}", username, ip);
                
                HttpContext.Session.SetString("User", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante el proceso de autenticación del usuario {Username}", username);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Login (POST) | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
        
        public IActionResult Dashboard()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Dashboard | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue) 
                {
                    _logger.LogWarning("Intento de acceso al Dashboard sin autenticar desde la IP {IP}", ip);
                    return RedirectToAction("Login");
                }
                
                var user = _db.Users.Find(userId.Value);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el Dashboard para el usuario {Usuario}", usuario);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Dashboard | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
        
        public IActionResult Logout()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio AuthController.Logout | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                _logger.LogInformation("EVENTO AUTH: El usuario '{Usuario}' cerró sesión desde la IP {IP}", usuario, ip);
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el cierre de sesión del usuario {Usuario}", usuario);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin AuthController.Logout | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}