﻿using Actio.Common.Events;
using Actio.Common.Services;

namespace Actio.Api
{
   public class Program
   {
      public static void Main(string[] args)
      {
         ServiceHost.Create<Startup>(args)
         .UseRabbitMq()
         .SubscribeToEvent<ActivityCreated>()
         .Build()
         .Run();



         //       CreateWebHostBuilder(args).Build().Run();
         //   }

         //   public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
         //       WebHost.CreateDefaultBuilder(args)
         //           .UseStartup<Startup>();
      }
   }
}