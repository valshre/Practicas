using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web; // Librería necesaria para el HtmlEncoder
using System.Collections.Generic;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        // Se mantiene la lista estática en memoria según tu Actividad 1 y 3
        private static List<string> _comments = new();

        public IActionResult Index()
        {
            return View(_comments);
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
        
            if (!string.IsNullOrEmpty(comment))
            {
                // converte cualquier etiqueta  script en texto inofensivo
                string safeComment = HtmlEncoder.Default.Encode(comment);
            
                _comments.Add(safeComment);
            }
            
            return RedirectToAction("Index");
        }
    }
public class CommentController : Controller
{
private static List<string> _comments = new();
public IActionResult Index()
{
return View(_comments);
}
[HttpPost]
public IActionResult AddComment(string comment)
{
if (!string.IsNullOrEmpty(comment))
{
_comments.Add(comment);
}
return RedirectToAction("Index");
}
}