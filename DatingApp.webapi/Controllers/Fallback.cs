using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.webapi.Controllers
{
    [AllowAnonymous]
    public class Fallback: Controller
    {
        public IActionResult Index()
        {
            return new PhysicalFileResult(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }        
    }
}