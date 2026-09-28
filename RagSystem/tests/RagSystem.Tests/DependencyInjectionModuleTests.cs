using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RagSystem.Application;
using RagSystem.Infrastructure;
using Xunit;

namespace RagSystem.Tests;

public class DependencyInjectionModuleTests
{
    [Fact]
    public void AddApplication_Should_ReturnSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructure_Should_ReturnSameServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        var result = services.AddInfrastructure(configuration);

        Assert.Same(services, result);
    }
}
