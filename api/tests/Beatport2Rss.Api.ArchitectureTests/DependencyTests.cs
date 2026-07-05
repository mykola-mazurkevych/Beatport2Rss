using NetArchTest.Rules;

using Xunit;

namespace Beatport2Rss.Api.ArchitectureTests;

public sealed class DependencyTests : Base.Tests
{
    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    // [Fact]
    // public void Domain_ShouldNotHaveDependencyOn_Api()
    // {
    //     var result = Types.InAssembly(DomainAssembly)
    //         .Should()
    //         .NotHaveDependencyOn(ApiAssembly.GetName().Name)
    //         .GetResult();

    //     Assert.True(result.IsSuccessful);
    // }

    [Fact]
    public void Application_ShouldNotHaveDependencyOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    // [Fact]
    // public void Application_ShouldNotHaveDependencyOn_Api()
    // {
    //     var result = Types.InAssembly(ApplicationAssembly)
    //         .Should()
    //         .NotHaveDependencyOn(ApiAssembly.GetName().Name)
    //         .GetResult();

    //     Assert.True(result.IsSuccessful);
    // }

    // [Fact]
    // public void Infrastructure_ShouldNotHaveDependencyOn_Api()
    // {
    //     var result = Types.InAssembly(InfrastructureAssembly)
    //         .Should()
    //         .NotHaveDependencyOn(ApiAssembly.GetName().Name)
    //         .GetResult();

    //     Assert.True(result.IsSuccessful);
    // }
}