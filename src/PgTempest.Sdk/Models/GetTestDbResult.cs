namespace PgTempest.Sdk.Models;

public sealed record GetTestDbResult(
    TestDbId TestDbId,
    DbConnectionOptions DbConnectionOptions,
    DateTime UsageDeadline
);
