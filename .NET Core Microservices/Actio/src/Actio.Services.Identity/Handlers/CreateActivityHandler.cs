using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using RawRabbit;

namespace Actio.Services.Identity.Handlers
{
   public class CreateActivityHandler : ICommandHandler<CreateActivity>
   {
      private readonly IBusClient _busClient;
      public CreateActivityHandler(IBusClient busClient)
      {
         this._busClient = busClient;
      }

      public async Task HandleAsync(CreateActivity command)
      {
         Console.WriteLine($"CreateActivityHandler : {command.Name}");
         await this._busClient.PublishAsync(new ActivityCreated(command.Id, command.UserId, command.Category, command.Name));
      }
   }
}