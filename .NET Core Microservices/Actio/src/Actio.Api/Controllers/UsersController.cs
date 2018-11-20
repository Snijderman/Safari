using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace Actio.Api.Controllers
{
   [Route("[controller]")]
   public class UsersController : Controller
   {
      private readonly IBusClient _busClient;
      public UsersController(IBusClient busClient)
      {
         this._busClient = busClient;
      }

      [HttpPost("register")]
      public async Task<IActionResult> Post([FromBody]CreateUser command)
      {
         await this._busClient.PublishAsync(command);

         return Accepted();
      }

   }
}