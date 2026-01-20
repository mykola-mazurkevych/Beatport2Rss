using Beatport2Rss.Domain.Common.Interfaces;

using NetArchTest.Rules;

namespace Beatport2Rss.ArchitectureTests;

public sealed class DomainTests : Base.Tests
{
    [Fact]
    public void ValueObject_ShouldNotHave_PublicContructors() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetConstructors(), c => c.IsPublic));

    [Fact]
    public void ValueObject_Properties_ShouldNotHave_PublicSetters() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.DoesNotContain(t.GetProperties(), p => p.SetMethod is not null && p.SetMethod.IsPublic));

    [Fact]
    public void ValueObject_ShouldHave_PublicStaticCreateMethod() =>
        GetValueObjectTypes()
            .ForEach(t => Assert.Contains(t.GetMethods(), m => m is { IsStatic: true, IsPublic: true, Name: "Create" }));

    private static List<Type> GetValueObjectTypes() =>
        Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IValueObject))
            .GetTypes()
            .ToList();
}