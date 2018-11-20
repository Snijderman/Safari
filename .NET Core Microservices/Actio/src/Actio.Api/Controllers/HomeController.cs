using Microsoft.AspNetCore.Mvc;

namespace Actio.Api.Controllers
{
   [Route("")]
   public class HomeController : Controller
   {
      [HttpGet("")]
      public IActionResult Get() => Content("Hallo vanuit de API!");
   }
}