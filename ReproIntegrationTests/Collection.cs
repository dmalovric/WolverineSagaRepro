using FastEndpoints.Testing;

namespace ReproIntegrationTests;

[CollectionDefinition(Name)]
public class TestCollection : TestCollection<Fixture>
{
    public const string Name = nameof(TestCollection);
}

