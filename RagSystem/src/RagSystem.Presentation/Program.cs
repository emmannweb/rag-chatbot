using RagSystem.Application;
using RagSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

// 1. Add CORS service configuration here
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Allow Vite and standard React ports
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "RAG System API"; // Your custom title
        document.Info.Version = "v1";
        document.Info.Description = " RAG System project with clean architecture.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// 2. Enable CORS middleware here (must be before MapControllers)
app.UseCors("AllowReactFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // Must match the URL you just tested manually
        options.SwaggerEndpoint("/openapi/v1.json", "RAG System API V1");

        // This matches your launchSettings.json "api-docs"
        options.RoutePrefix = "api-docs";
    });
}

app.MapControllers();

app.Run();
