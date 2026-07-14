using System.Reflection;

using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.ArchitectureTests.Base;

public abstract class Tests
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Slug).Assembly; // TODO: change;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(Program).Assembly;
}