using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.webapi.Controllers
{
    public class Fallback: Controller
    {
        public IActionResult Index()
        {
            return new PhysicalFileResult(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }        
    }
}