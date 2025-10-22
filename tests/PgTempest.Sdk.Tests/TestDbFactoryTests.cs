using PgTempest.Sdk.Client;
using PgTempest.Sdk.Factory;
using PgTempest.Sdk.Tests.Extensions;

namespace PgTempest.Sdk.Tests;

public sealed class TestDbFactoryTests
{
    [Test]
    public async Task CreateTestDb_TemplateIsInitialized()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());
        var templateHash = Random.Shared.NextTemplateHash();
        var initializationDuration = TimeSpan.FromSeconds(10);
        var usageDuration = TimeSpan.FromSeconds(10);

        await client.InitializeTemplate(
            templateHash,
            initializationDuration,
            _ => Task.CompletedTask
        );

        var factory = new TestDbFactory(client, templateHash);

        // Act
        await using var testDbUsageGuard = await factory.CreateTestDb(usageDuration);
    }
}
