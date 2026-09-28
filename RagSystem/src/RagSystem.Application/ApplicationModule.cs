using Microsoft.Extensions.DependencyInjection;
using RagSystem.Application.UseCases.Rag.Chat;
using RagSystem.Application.UseCases.Rag.UploadDocuments;

namespace RagSystem.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUseCases();
        return services;
    }

    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Register application use cases here as the project grows.
        services.AddScoped<IUploadDocumentsUseCase, UploadDocumentsUseCase>();
        services.AddScoped<IChatUseCase, ChatUseCase>();
        return services;
    }
}
