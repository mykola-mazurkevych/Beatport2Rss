using System.Reflection;

namespace Beatport2Rss.ArchitectureTests.Base;

public abstract class ArchitectureTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Domain.Common.Interfaces.IAggregateRoot<>).Assembly; // TODO: change;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.ServiceCollectionExtensions).Assembly;
    protected static readonly Assembly SharedKernelAssembly = typeof(SharedKernel.Errors.ConflictError).Assembly; // TODO: change;
    protected static readonly Assembly WebApiAssembly = typeof(Program).Assembly;
}