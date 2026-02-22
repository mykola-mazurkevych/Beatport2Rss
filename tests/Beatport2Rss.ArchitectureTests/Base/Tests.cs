using System.Reflection;

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.ArchitectureTests.Base;

public abstract class Tests
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Slug).Assembly; // TODO: change;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly SharedKernelAssembly = typeof(IAggregateRoot<>).Assembly; // TODO: change;
    protected static readonly Assembly WebApiAssembly = typeof(Program).Assembly;
}