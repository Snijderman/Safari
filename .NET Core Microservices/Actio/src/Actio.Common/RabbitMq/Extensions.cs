using System;
using System.Reflection;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RawRabbit;
using RawRabbit.Instantiation;

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

      private static string GetQueueName<T>()
      {
         return $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
      }

      public static void AddRabbitMq(this IServiceCollection service, IConfiguration configuration)
      {
         RabbitMqOptions options = new RabbitMqOptions();
         var section = configuration.GetSection("RabbitMq"); // <= vult options met section uit appsettings.json
         section.Bind(options);

         //var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
         //{
         //   ClientConfiguration = options
         //});

         //service.AddSingleton<IBusClient>(_ => TestManual());
         TestManual();


         //var clientFactory = BusClientFactory.CreateDefault(GetRawRabbitConfiguration());

         var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
         {
            ClientConfiguration = GetRawRabbitConfiguration()
         });

         service.AddSingleton<IBusClient>(_ => client);
      }

      private static IConnection TestManual()
      {
         var connectionFactory = new ConnectionFactory();
         connectionFactory.HostName = "sheep.rmq.cloudamqp.com";
         connectionFactory.VirtualHost = "couxaoun";
         connectionFactory.UserName = "couxaoun";
         connectionFactory.Password = "y9cAOE8tJJYwG-dU55Zwm9q796dWWtX5";

         return connectionFactory.CreateConnection();
      }

      private static RawRabbit.Configuration.RawRabbitConfiguration GetRawRabbitConfiguration()
      {
         /*
         "sheep.rmq.cloudamqp.com",
         "sheep-01.rmq.cloudamqp.com"

          */

         RawRabbit.Configuration.RawRabbitConfiguration config = new RawRabbit.Configuration.RawRabbitConfiguration();
         config.Hostnames.Add("http://sheep.rmq.cloudamqp.com");
         config.Hostnames.Add("tcp://sheep-01.rmq.cloudamqp.com");
         config.Hostnames.Add("amqp://sheep-01.rmq.cloudamqp.com");
         config.VirtualHost = "couxaoun";
         config.Username = "couxaoun";
         config.Password = "y9cAOE8tJJYwG-dU55Zwm9q796dWWtX5";
         config.PersistentDeliveryMode = true;
         config.AutoCloseConnection = true;
         config.AutomaticRecovery = true;
         config.TopologyRecovery = true;
         config.Exchange = new RawRabbit.Configuration.GeneralExchangeConfiguration
         {
            Durable = true,
            AutoDelete = true,
            Type = RawRabbit.Configuration.Exchange.ExchangeType.Topic
         };
         config.Queue = new RawRabbit.Configuration.GeneralQueueConfiguration
         {
            AutoDelete = true,
            Durable = true,
            Exclusive = true
         };


         /*
   "PersistentDeliveryMode": true,
   "AutoCloseConnection": true,
   "AutomaticRecovery": true,
   "TopologyRecovery": true,
   "Exchange": {
      "Durable": true,
      "AutoDelete": true,
      "Type": "Topic"
   },
   "Queue": {
      "AutoDelete": true,
      "Durable": true,
      "Exclusive": true
          * */


         //config.Port = 1883;

         //const string url = @"amqp://couxaoun:y9cAOE8tJJYwG-dU55Zwm9q796dWWtX5@sheep.rmq.cloudamqp.com/couxaoun";
         //var tst = new AmqpTcpEndpoint(new Uri(url));

         return config;
      }
   }
}