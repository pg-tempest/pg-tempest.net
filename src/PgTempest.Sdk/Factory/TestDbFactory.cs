using PgTempest.Sdk.Client;
using PgTempest.Sdk.Models;

namespace PgTempest.Sdk.Factory;

public sealed class TestDbFactory(PgTempestClient client, TemplateHash templateHash)
{
    public async Task<TestDbUsageGuard> CreateTestDb(
        TimeSpan usageDuration,
        CancellationToken cancellationToken = default
    )
    {
        var getTestDbResult = await client.GetTestDb(
            templateHash,
            usageDuration,
            cancellationToken
        );

        return new TestDbUsageGuard(
            client,
            templateHash,
            getTestDbResult.TestDbId,
            getTestDbResult.DbConnectionOptions
        );
    }
}
