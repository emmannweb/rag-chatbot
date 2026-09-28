using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RagSystem.Domain.Repositories;
using RagSystem.Infrastructure.AI.Ollama.OllamaProvider;
using RagSystem.Infrastructure.Persistence;
using RagSystem.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;
using OllamaSharp;
using RagSystem.Infrastructure.AI.Ollama.Configuration;

namespace RagSystem.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddServices(configuration);
        return services;
    }

    public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration
        )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseVector() // <--- Enables pgvector type mapping in Npgsql
            )
        );

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Repository registration will be added here.
        services.AddScoped<IRagRepository, RagRepository>();
        services.AddScoped<IAIProvider, OllamaAiProvider>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure service registration will be added here.
        // Bind and register OllamaSettings
        services.Configure<OllamaSettings>(configuration.GetSection(OllamaSettings.SectionName));

        // Register the OllamaSharp API Client with an extended timeout (5 minutes)
        services.AddScoped<IOllamaApiClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.BaseUrl),
                Timeout = TimeSpan.FromMinutes(5) // Prevents local 100-second timeout drops
            };

            return new OllamaApiClient(httpClient);
        });

        return services;
    }
}
