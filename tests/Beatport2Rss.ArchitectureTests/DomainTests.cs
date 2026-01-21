using Beatport2Rss.Domain.Common.Interfaces;

using NetArchTest.Rules;

namespace Beatport2Rss.ArchitectureTests;

public sealed class DomainTests : Base.Tests
{
    [Fact]
    public void Entity_ShouldNotHave_PublicConstructor() =>
        GetEntityTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetConstructors(), c => c.IsPublic));
    
    [Fact]
    public void Entity_Property_ShouldNotHave_PublicSetter() =>
        GetEntityTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetProperties(), p => p.SetMethod is not null && p.SetMethod.IsPublic));
    
    [Fact]
    public void Entity_ShouldHave_PublicStaticCreateMethod() =>
        GetEntityTypes()
            .ForEach(t => Assert.Contains(t.GetMethods(), m => m is { IsStatic: true, IsPublic: true, Name: "Create" }));

    [Fact]
    public void ValueObject_ShouldNotHave_PublicContructor() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetConstructors(), c => c.IsPublic));

    [Fact]
    public void ValueObject_Property_ShouldNotHave_PublicSetter() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetProperties(), p => p.SetMethod is not null && p.SetMethod.IsPublic));

    [Fact]
    public void ValueObject_ShouldHave_PublicStaticCreateMethod() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.Contains(t.GetMethods(), m => m is { IsStatic: true, IsPublic: true, Name: "Create" }));

    private static List<Type> GetEntityTypes() =>
        Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IEntity<>))
            .And()
            .AreNotInterfaces()
            .GetTypes()
            .ToList();

    private static List<Type> GetValueObjectTypes() =>
        Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IValueObject))
            .And()
            .AreNotInterfaces()
            .GetTypes()
            .ToList();
}