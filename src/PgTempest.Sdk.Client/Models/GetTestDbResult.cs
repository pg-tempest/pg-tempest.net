namespace PgTempest.Sdk.Client.Models;

public sealed record GetTestDbResult(
    TestDbId TestDbId,
    DbConnectionOptions DbConnectionOptions,
    DateTime UsageDeadline
);
