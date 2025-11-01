using Npgsql;
using PgTempest.Sdk.Client;
using PgTempest.Sdk.Factory;
using PgTempest.Sdk.Models;

namespace Samples.Dupper.Tests.NUnit;

[SetUpFixture]
internal sealed class DbFactoryFixture
{
    public static readonly string PgTempestHost =
        Environment.GetEnvironmentVariable("PG_TEMPEST_HOST") ?? "localhost";
    public static readonly string PgTempestPort =
        Environment.GetEnvironmentVariable("PG_TEMPEST_PORT") ?? "8000";

    public static TestDbFactory TestDbFactory { get; private set; }

    [OneTimeSetUp]
    public static async Task Setup()
    {
        var templateHash = TemplateHash.Calculate(
            Migrator.GetMigrations().Select(x => x.Name + x.Content)
        );

        var pgTempestClient = PgTempestClient.NewFromBaseUrl(
            $"http://{PgTempestHost}:{PgTempestPort}"
        );

        await pgTempestClient.InitializeTemplate(
            templateHash,
            initializationDuration: TimeSpan.FromSeconds(30),
            initializationCallback: async dbConnectionOptions =>
            {
                await using var npgsqlDataSource = NpgsqlDataSource.Create(
                    dbConnectionOptions.ConnectionString
                );

                var migrator = new Migrator(npgsqlDataSource);
                await migrator.MigrateAsync();
            }
        );

        TestDbFactory = new TestDbFactory(pgTempestClient, templateHash);
    }
}
