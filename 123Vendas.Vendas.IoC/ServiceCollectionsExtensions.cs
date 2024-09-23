using _123Vendas.Vendas.Data.Context;
using _123Vendas.Vendas.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;
using _123Vendas.Vendas.Infra.Interface.Repository;
using _123Vendas.Vendas.Infra.Interface.Events;
using _123Vendas.Vendas.Service.Events;
using _123Vendas.Vendas.Infra.Interface.Service;
using _123Vendas.Vendas.Service.Services;

namespace _123Vendas.Vendas.IoC
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura o DbContext
            services.AddDbContext<SalesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SalesDbConnection")));

            // Registra o repositório e a interface
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

            return services;
        }
    }
}
