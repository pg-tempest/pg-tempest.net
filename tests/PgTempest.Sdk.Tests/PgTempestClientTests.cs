using PgTempest.Sdk.Client;
using PgTempest.Sdk.Models;
using PgTempest.Sdk.Tests.Extensions;
using Shouldly;

namespace PgTempest.Sdk.Tests;

[Parallelizable(ParallelScope.All)]
public sealed class PgTempestClientTests
{
    [Test]
    public async Task StartTemplateInitialization_TwoCalls()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var initializationDuration = TimeSpan.FromSeconds(10);
        var minExpectedInitializationDeadline = now;
        var maxExpectedInitializationDeadline =
            now + initializationDuration + TimeSpan.FromSeconds(10);

        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        // Act
        var firstResult = await client.StartTemplateInitialization(
            templateHash,
            initializationDuration
        );

        // Assert
        var startedResult = firstResult.UnwrapInitializationWasStarted();
        startedResult.DbConnectionOptions.Database.ShouldContain(templateHash.ToString());
        startedResult.InitializationDeadline.ShouldBeInRange(
            minExpectedInitializationDeadline,
            maxExpectedInitializationDeadline
        );

        // Act
        var secondResult = await client.StartTemplateInitialization(
            templateHash,
            initializationDuration
        );

        // Assert
        var inProgressResult = secondResult.UnwrapInitializationIsInProgress();
        inProgressResult.InitializationDeadline.ShouldBeInRange(
            minExpectedInitializationDeadline,
            maxExpectedInitializationDeadline
        );
    }

    [Test]
    public async Task FinishTemplateInitialization_TemplateWasNotFound()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        // Act
        var action = () => client.FinishTemplateInitialization(templateHash);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task FinishTemplateInitialization_TemplateIsInProgress()
    {
        // Arrange
        var initializationDuration = TimeSpan.FromSeconds(10);

        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        await client.StartTemplateInitialization(templateHash, initializationDuration);

        // Act
        await client.FinishTemplateInitialization(templateHash);
    }

    [Test]
    public async Task FinishTemplateInitialization_InitializationIsFailed()
    {
        // Arrange
        var initializationDuration = TimeSpan.FromSeconds(10);

        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        await client.StartTemplateInitialization(
            templateHash,
            initializationDuration,
            CancellationToken.None
        );

        await client.MarkTemplateInitializationAsFailed(templateHash);

        // Act
        var action = () => client.FinishTemplateInitialization(templateHash);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GetTestDb_TemplateWasNotFound()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var usageDuration = TimeSpan.FromSeconds(10);
        var templateHash = Random.Shared.NextTemplateHash();

        // Act
        var action = () => client.GetTestDb(templateHash, usageDuration);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GetTestDb_TemplateIsNotInitialized()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var initializationDuration = TimeSpan.FromSeconds(10);
        var usageDuration = TimeSpan.FromSeconds(10);
        var templateHash = Random.Shared.NextTemplateHash();

        await client.StartTemplateInitialization(templateHash, initializationDuration);

        // Act
        var action = () => client.GetTestDb(templateHash, usageDuration);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GetTestDb_TemplateIsInitialized()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var initializationDuration = TimeSpan.FromSeconds(10);
        var usageDuration = TimeSpan.FromSeconds(10);
        var minExpectedUsageDeadline = now;
        var maxExpectedUsageDeadline = now + usageDuration + TimeSpan.FromSeconds(10);
        var templateHash = Random.Shared.NextTemplateHash();

        await client.StartTemplateInitialization(templateHash, initializationDuration);
        await client.FinishTemplateInitialization(templateHash);

        // Act
        var getTestDbResult = await client.GetTestDb(templateHash, usageDuration);

        // Assert
        getTestDbResult.DbConnectionOptions.Database.ShouldContain(templateHash.ToString());
        getTestDbResult.DbConnectionOptions.Database.ShouldContain(templateHash.ToString());
        getTestDbResult.TestDbId.Value.ShouldNotBe((ushort)0);
        getTestDbResult.UsageDeadline.ShouldBeInRange(
            minExpectedUsageDeadline,
            maxExpectedUsageDeadline
        );
    }

    [Test]
    public async Task ReleaseTestDb_TemplateWasNotFound()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        // Act
        var action = () => client.ReleaseTestDb(templateHash, new TestDbId(1));

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task ReleaseTestDb_TestDbWasNotFound()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        var initializationDuration = TimeSpan.FromSeconds(10);
        var usageDuration = TimeSpan.FromSeconds(10);
        await client.StartTemplateInitialization(templateHash, initializationDuration);
        await client.FinishTemplateInitialization(templateHash);

        // Act
        var action = () => client.ReleaseTestDb(templateHash, new TestDbId(1));

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task ReleaseTestDb_Ok()
    {
        // Arrange
        var client = new PgTempestClient(PgTempestContainerFixture.GetPgTempestHttpClient());

        var templateHash = Random.Shared.NextTemplateHash();

        var initializationDuration = TimeSpan.FromSeconds(10);
        var usageDuration = TimeSpan.FromSeconds(10);
        await client.StartTemplateInitialization(templateHash, initializationDuration);
        await client.FinishTemplateInitialization(templateHash);
        var getTestDbResult = await client.GetTestDb(templateHash, usageDuration);

        // Act
        await client.ReleaseTestDb(templateHash, getTestDbResult.TestDbId);
    }
}
