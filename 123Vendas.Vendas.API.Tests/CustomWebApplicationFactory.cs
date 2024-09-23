using _123Vendas.Vendas.Data.Context;
using _123Vendas.Vendas.Data.Repository;
using _123Vendas.Vendas.Infra.Interface.Events;
using _123Vendas.Vendas.Infra.Interface.Repository;
using _123Vendas.Vendas.Service.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using System.Net.Http;
using _123Vendas.Vendas.Infra.Interface.Service;
using _123Vendas.Vendas.Service.Services;

namespace _123Vendas.Vendas.API.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(SalesDbContext));
                if (context != null)
                {
                    services.Remove(context);
                    var options = services.Where(r => r.ServiceType == typeof(DbContextOptions)
                      || r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)).ToArray();
                    foreach (var option in options)
                    {
                        services.Remove(option);
                    }
                }

                // Add a new registration for ApplicationDbContext with an in-memory database
                services.AddDbContext<SalesDbContext>(options =>
                {
                    // Provide a unique name for your in-memory database
                    options.UseInMemoryDatabase("InMemorySalesDbContext");
                });

                services.AddScoped<ISalesRepository, SalesRepository>();

                services.AddScoped<IEventPublisher, EventPublisher>();
                services.AddScoped<ISalesService, SalesService>();

                // Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();

                services.AddLogging(loggingBuilder =>
                    loggingBuilder.AddSerilog(dispose: true));

            });
        }
    }
}
