using Actio.Common.Commands;
using Actio.Common.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Instantiation;
using System.Reflection;
using System.Threading.Tasks;

namespace Actio.Common.RabbitMq
{
   public static class Extensions
   {
      public static Task WithCommandHandlerAsync<TCommand>(this IBusClient bus, ICommandHandler<TCommand> handler) where TCommand : ICommand
      {
         return bus.SubscribeAsync<TCommand>(msg => handler.HandleAsync(msg),
         ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>()))));
      }

      public static Task WithEventHandlerAsync<TEvent>(this IBusClient bus, IEventHandler<TEvent> handler) where TEvent : IEvent
      {
         return bus.SubscribeAsync<TEvent>(msg => handler.HandleAsync(msg),
         ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>()))));
      }

      private static string GetQueueName<T>() => $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";

      public static void AddRabbitMq(this IServiceCollection service, IConfiguration configuration)
      {
         var options = new RabbitMqOptions();
         var section = configuration.GetSection("RabbitMq"); // <= vult options met section uit appsettings.json
         section.Bind(options);

         //var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
         //{
         //   ClientConfiguration = options
         //});

         var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
         {
            ClientConfiguration = GetRawRabbitConfiguration()
         });

         service.AddSingleton<IBusClient>(_ => client);
      }

      private static RawRabbit.Configuration.RawRabbitConfiguration GetRawRabbitConfiguration()
      {
         var config = new RawRabbit.Configuration.RawRabbitConfiguration();
         config.Hostnames.Add("sheep.rmq.cloudamqp.com");
         config.VirtualHost = "couxaoun";
         config.Username = "couxaoun";
         config.Password = "y9cAOE8tJJYwG-dU55Zwm9q796dWWtX5";

         return config;
      }
   }
}