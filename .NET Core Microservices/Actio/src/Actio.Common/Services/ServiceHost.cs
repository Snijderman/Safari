using System;
using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.RabbitMq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RawRabbit;

namespace Actio.Common.Services
{
   public class ServiceHost : IServiceHost
   {
      private readonly IWebHost _webHost;

      public ServiceHost(IWebHost webhost)
      {
         this._webHost = webhost;
      }

      public void Run() => this._webHost.Run();

      public static HostBuilder Create<TStartup>(string[] args) where TStartup : class
      {
         Console.Title = typeof(TStartup).Namespace;
         var config = new ConfigurationBuilder()
         .AddEnvironmentVariables()
         .AddCommandLine(args)
         .Build();

         var webhostBullder = WebHost.CreateDefaultBuilder()
         .UseConfiguration(config)
         .UseStartup<TStartup>();

         return new HostBuilder(webhostBullder.Build());
      }
   }

   public abstract class BuilderBase
   {
      public abstract ServiceHost Build();
   }

   public class HostBuilder : BuilderBase
   {
      private readonly IWebHost _webHost;
      private IBusClient _bus;

      public HostBuilder(IWebHost webhost)
      {
         this._webHost = webhost;
      }

      public override ServiceHost Build()
      {
         return new ServiceHost(this._webHost);
      }

      public BusBuilder UseRabbitMq()
      {
         this._bus = (IBusClient)this._webHost.Services.GetService(typeof(IBusClient));

         return new BusBuilder(this._webHost, this._bus);
      }
   }

   public class BusBuilder : BuilderBase
   {
      private readonly IWebHost _webHost;
      private IBusClient _bus;

      public BusBuilder(IWebHost webhost, IBusClient busClient)
      {
         this._webHost = webhost;
         this._bus = busClient;
      }

      public BusBuilder SubscribeToCommand<TCommand>() where TCommand : ICommand
      {
         var handler = (ICommandHandler<TCommand>)this._webHost.Services.GetService(typeof(ICommandHandler<TCommand>));

         this._bus.WithCommandHandlerAsync(handler);

         return this;
      }

      public BusBuilder SubscribeToEvent<TEvent>() where TEvent : IEvent
      {
         var handler = (IEventHandler<TEvent>)this._webHost.Services.GetService(typeof(IEventHandler<TEvent>));

         this._bus.WithEventHandlerAsync(handler);

         return this;
      }

      public override ServiceHost Build()
      {
         return new ServiceHost(this._webHost);
      }
   }
}