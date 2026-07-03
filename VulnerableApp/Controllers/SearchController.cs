using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using VulnerableApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger; 

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio SearchController.Index | Usuario: {Usuario} | IP: {IP} | Término Búsqueda: {Search}", usuario, ip, search);

            try
            {
               
                if (string.IsNullOrEmpty(search))
                {
                    return View(new List<User>());
                }

                // Genera un Warning si alguien busca cadenas muy largas que puedan ser intentos de inyección
                if(search.Length > 50)
                {
                    _logger.LogWarning("Búsqueda sospechosamente larga por el usuario {Usuario} desde la IP {IP}", usuario, ip);
                }

                var users = _db.Users
                    .Where(u => u.Username.Contains(search))
                    .ToList();

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SearchController.Index buscando '{Search}' por usuario {Usuario}", search, usuario);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin SearchController.Index | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}