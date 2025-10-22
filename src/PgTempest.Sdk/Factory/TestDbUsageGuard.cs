using PgTempest.Sdk.Client;
using PgTempest.Sdk.Models;

namespace PgTempest.Sdk.Factory;

public sealed class TestDbUsageGuard(
    PgTempestClient client,
    TemplateHash templateHash,
    TestDbId testDbId,
    DbConnectionOptions connectionOptions
) : IAsyncDisposable
{
    public TemplateHash TemplateHash => templateHash;
    public TestDbId TestDbId => testDbId;
    public DbConnectionOptions ConnectionOptions => connectionOptions;

    public async ValueTask DisposeAsync()
    {
        await client.ReleaseTestDb(templateHash, testDbId);
    }
}
