using Franz.Common.EntityFramework.SQLServer.Extensions;
using Franz.Common.Messaging.Bootstrap.Extenstions;
using Franz.Common.Messaging.Delegating;

using ItemService.Consumer.Extensions;
using ItemService.Consumer.Handlers;

using ItemService.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Franz.Common.Messaging.Kafka.Extensions;
namespace ItemService.Consumer
{
  public static class Program
  {
    public static async Task Main(string[] args)
    {
      using IHost host = Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((context, config) =>
          {
            // Load base configuration
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
          })
          .ConfigureServices((context, services) =>
          {
            var configuration = context.Configuration;
            var environment = context.HostingEnvironment;

            // Ã°Å¸Â§Â© Messaging architecture setup (Kafka + Delegating)
            services.AddMessagingArchitecture(environment, configuration);
            services.AddKafkaMessaging(configuration);

            // Ã°Å¸â€”â€žÃ¯Â¸Â Database setup (SQL Server)
            services.AddSqlServerDatabase<ApplicationDbContext>(configuration);

            // Ã°Å¸Â§Â  Business domain and consumer services
           // services.AddScoped<IMessageHandler, KafkaMessageHandler>();
            services.AddKafkaMessagingServices(configuration); // from ItemService.Consumer.Extensions
          })
          .Build();

      // Ã¢Å“â€¦ Start host (graceful lifetime)
      await host.RunAsync();
    }
  }
}



